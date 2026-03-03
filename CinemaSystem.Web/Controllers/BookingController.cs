using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CinemaSystem.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult SelectSeats(int showtimeId)
        {
            // 1. CRITICAL FIX: Added CinemaHall.Cinema to the include properties
            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema,CinemaHall.Seats"
            );

            if (showtime == null) return NotFound("Showtime not found.");

            // 2. Safely extract seats (prevent null reference if Hall has no seats yet)
            var hallSeats = showtime.CinemaHall?.Seats ?? new List<Seat>();

            var bookedSeatIds = _unitOfWork.Ticket?.GetAll(t => t.Booking.ShowtimeId == showtimeId)
                                    .Select(t => t.SeatId).ToList() ?? new List<int>();

            var heldSeatIds = _unitOfWork.SeatHold.GetAll(h =>
                                    h.ShowtimeId == showtimeId &&
                                    h.HoldExpiration > DateTime.Now)
                                    .Select(h => h.SeatId).ToList();

            // 3. Construct the ViewModel safely
            var vm = new SeatSelectionVM
            {
                Showtime = showtime,
                Rows = hallSeats.Any() ? hallSeats.Max(s => s.Row[0] - 64) : 0,
                Cols = hallSeats.Any() ? hallSeats.Max(s => s.Column) : 0,
                Seats = new List<SeatWithStatusDto>() // Explicitly initialize to prevent View crashes
            };

            // 4. Map the seats
            foreach (var seat in hallSeats)
            {
                string currentStatus = "Available";
                if (bookedSeatIds.Contains(seat.Id)) currentStatus = "Booked";
                else if (heldSeatIds.Contains(seat.Id)) currentStatus = "Held";

                vm.Seats.Add(new SeatWithStatusDto
                {
                    Id = seat.Id,
                    Row = seat.Row ?? "", // Fallback empty string
                    Column = seat.Column,
                    SeatType = seat.SeatType,
                    IsAccessible = seat.IsAccessible,
                    FinalPrice = PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, seat.SeatType),
                    Status = currentStatus
                });
            }

            return View(vm);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken] // Note: Requires Program.cs configuration to work with AJAX JSON headers
        [Authorize]
        public IActionResult LockSeatsAjax([FromBody] HoldSeatsRequestDto dto)
        {
            // 1. Basic Validation
            if (dto == null || dto.SelectedSeatIds == null || !dto.SelectedSeatIds.Any())
            {
                return Json(new { success = false, message = "No seats selected." });
            }

            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 3. Database Integrity Check 1: Are they already sold?
            var bookedSeats = _unitOfWork.Ticket?.GetAll(t =>
                t.Booking.ShowtimeId == dto.ShowtimeId &&
                dto.SelectedSeatIds.Contains(t.SeatId)).ToList() ?? new List<Ticket>();

            if (bookedSeats.Any())
            {
                return Json(new { success = false, message = "Transaction failed: One or more seats have already been purchased." });
            }

            // 4. Database Integrity Check 2: Are they currently held by someone else?
            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == dto.ShowtimeId &&
                dto.SelectedSeatIds.Contains(h.SeatId) &&
                h.HoldExpiration > DateTime.Now).ToList();

            if (activeHolds.Any(h => h.ApplicationUserId != userId))
            {
                return Json(new { success = false, message = "Transaction failed: Another user is currently checking out with these seats." });
            }

            // 5. Cleanup: Prevent "Hoarding"
            // If this specific user clicks back and forth generating multiple holds, wipe their old ones for this showtime.
            var userExistingHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == dto.ShowtimeId && h.ApplicationUserId == userId).ToList();

            if (userExistingHolds.Any())
            {
                _unitOfWork.SeatHold.RemoveRange(userExistingHolds);
            }

            // 6. The Lock Application
            var expirationTime = DateTime.Now.AddMinutes(10); // 10-minute cart timer

            foreach (var seatId in dto.SelectedSeatIds)
            {
                _unitOfWork.SeatHold.Add(new SeatHold
                {
                    ShowtimeId = dto.ShowtimeId,
                    SeatId = seatId,
                    ApplicationUserId = userId,
                    HoldExpiration = expirationTime
                });
            }

            // 7. Commit Transaction
            _unitOfWork.Save();

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Checkout(int showtimeId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Validate Active Holds
            // We strictly pull holds belonging to THIS user, for THIS showtime, that have NOT expired.
            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == showtimeId &&
                h.ApplicationUserId == userId &&
                h.HoldExpiration > DateTime.Now,
                includeProperties: "Seat").ToList();

            // The Cart Timer check
            if (!activeHolds.Any())
            {
                TempData["error"] = "Your seat reservation has expired or no seats were selected. Please try again.";
                return RedirectToAction(nameof(SelectSeats), new { showtimeId = showtimeId });
            }

            // 3. Fetch Showtime Data for the UI
            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema"
            );

            if (showtime == null) return NotFound("Showtime details could not be loaded.");

            // 4. Build the ViewModel and calculate the Absolute Server-Side Truth
            var vm = new CheckoutVM
            {
                Showtime = showtime,
                TicketItems = new List<TicketItemDto>()
            };

            decimal runningTotal = 0;

            foreach (var hold in activeHolds)
            {
                // We recalculate the price here to prevent JavaScript tampering
                decimal seatPrice = PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, hold.Seat.SeatType);
                runningTotal += seatPrice;

                vm.TicketItems.Add(new TicketItemDto
                {
                    SeatId = hold.Seat.Id,
                    SeatName = $"{hold.Seat.Row}{hold.Seat.Column}",
                    Price = seatPrice
                });
            }

            vm.TotalAmount = runningTotal;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult FinalizeOrder(int showtimeId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Fetch the exact locks the user currently holds
            // We strictly check > DateTime.Now. If they clicked "Pay" at 10 minutes and 1 second, 
            // this returns empty and their transaction is safely rejected.
            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == showtimeId &&
                h.ApplicationUserId == userId &&
                h.HoldExpiration > DateTime.Now,
                includeProperties: "Seat").ToList();

            if (!activeHolds.Any())
            {
                TempData["error"] = "Your session expired or your seats were released. Please try again.";
                return RedirectToAction(nameof(SelectSeats), new { showtimeId = showtimeId });
            }

            // 2. Fetch Showtime Data for Pricing
            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie"
            );

            if (showtime == null) return NotFound();

            // 3. Recalculate Final Price (Never trust client data!)
            decimal totalAmount = 0;
            foreach (var hold in activeHolds)
            {
                totalAmount += PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, hold.Seat.SeatType);
            }

            // 4. Create the Parent Booking Record
            var newBooking = new Booking
            {
                ApplicationUserId = userId,
                ShowtimeId = showtimeId,
                Status = CinemaSystem.Models.Data.Enums.BookingStatus.Confirmed,
                PaymentStatus = CinemaSystem.Models.Data.Enums.PaymentStatus.Approved, // Simulating a successful payment
                TotalAmount = totalAmount,
                ConfirmationCode = "BK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                LoyaltyPointsEarned = (int)totalAmount, // Example: 1 point per dollar spent
                SessionId = "simulated_stripe_session_id",
                PaymentIntentId = "simulated_stripe_intent_id"
            };

            _unitOfWork.Booking.Add(newBooking);

            // We must save here to generate the newBooking.Id, which the Tickets need.
            // In EF Core, calling SaveChanges multiple times within the same web request 
            // is safe and handles its own local transaction.
            _unitOfWork.Save();

            // 5. Create the Line Items (Tickets)
            foreach (var hold in activeHolds)
            {
                var ticket = new Ticket
                {
                    BookingId = newBooking.Id, // Foreign Key linking to the parent
                    SeatId = hold.SeatId,
                    Price = PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, hold.Seat.SeatType), // Historical Snapshot
                    Barcode = Guid.NewGuid().ToString() // Unique ID for the QR code
                };
                _unitOfWork.Ticket.Add(ticket);
            }

            // 6. Delete the Temporary Holds
            // This removes the red "locked" status for other users and permanently marks the seat as "Booked"
            _unitOfWork.SeatHold.RemoveRange(activeHolds);

            // 7. Commit the Final Changes
            _unitOfWork.Save();

            // 8. Redirect to the Success Page
            return RedirectToAction(nameof(OrderConfirmation), new { bookingId = newBooking.Id });
        }

        [HttpGet]
        [Authorize]
        public IActionResult OrderConfirmation(int bookingId)
        {
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Fetch the Booking, enforcing the IDOR security check
            // We eager load the entire relational tree so the View has everything it needs to draw the tickets.
            var booking = _unitOfWork.Booking.Get(
                b => b.Id == bookingId && b.ApplicationUserId == userId,
                includeProperties: "Showtime,Showtime.Movie,Showtime.CinemaHall,Showtime.CinemaHall.Cinema,Tickets,Tickets.Seat"
            );

            // If the booking doesn't exist, OR if it belongs to someone else, reject them.
            if (booking == null)
            {
                return NotFound("Order not found or access denied.");
            }

            return View(booking);
        }
    }
}
