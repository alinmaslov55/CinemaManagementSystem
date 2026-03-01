using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Mvc;

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
        // [Authorize] // Uncomment when ASP.NET Core Identity is fully wired
        public IActionResult LockSeatsAjax([FromBody] HoldSeatsRequestDto dto)
        {
            // 1. Basic Validation
            if (dto == null || dto.SelectedSeatIds == null || !dto.SelectedSeatIds.Any())
            {
                return Json(new { success = false, message = "No seats selected." });
            }

            // 2. User Identification (Educational Concept)
            // We cannot lock a seat to "nobody". In production, we extract the ID from the logged-in user.
            // For testing right now, we will assign a temporary dummy string.
            string userId = "TEMP_GUEST_USER";
            // string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Use this later

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
    }
}
