using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CinemaSystem.Web.Controllers
{
    // OBJECTIVE FIX: Require authorization at the controller level to prevent anonymous UX crashes
    [Authorize]
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
            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema,CinemaHall.Seats"
            );

            if (showtime == null) return NotFound("Showtime not found.");

            var hallSeats = showtime.CinemaHall?.Seats ?? new List<Seat>();

            var bookedSeatIds = _unitOfWork.Ticket?.GetAll(t => t.Booking.ShowtimeId == showtimeId)
                                    .Select(t => t.SeatId).ToList() ?? new List<int>();

            var heldSeatIds = _unitOfWork.SeatHold.GetAll(h =>
                                    h.ShowtimeId == showtimeId &&
                                    h.HoldExpiration > DateTime.Now)
                                    .Select(h => h.SeatId).ToList();

            var vm = new SeatSelectionVM
            {
                Showtime = showtime,
                Rows = hallSeats.Any() ? hallSeats.Max(s => s.Row[0] - 64) : 0,
                Cols = hallSeats.Any() ? hallSeats.Max(s => s.Column) : 0,
                Seats = new List<SeatWithStatusDto>()
            };

            foreach (var seat in hallSeats)
            {
                string currentStatus = "Available";
                if (bookedSeatIds.Contains(seat.Id)) currentStatus = "Booked";
                else if (heldSeatIds.Contains(seat.Id)) currentStatus = "Held";

                vm.Seats.Add(new SeatWithStatusDto
                {
                    Id = seat.Id,
                    Row = seat.Row ?? "",
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
        [ValidateAntiForgeryToken] // OBJECTIVE FIX: Re-enabled. See instructions below for Program.cs.
        public IActionResult LockSeatsAjax([FromBody] HoldSeatsRequestDto dto)
        {
            if (dto == null || dto.SelectedSeatIds == null || !dto.SelectedSeatIds.Any())
            {
                return Json(new { success = false, message = "No seats selected." });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookedSeats = _unitOfWork.Ticket?.GetAll(t =>
                t.Booking.ShowtimeId == dto.ShowtimeId &&
                dto.SelectedSeatIds.Contains(t.SeatId)).ToList() ?? new List<Ticket>();

            if (bookedSeats.Any())
            {
                return Json(new { success = false, message = "Transaction failed: One or more seats have already been purchased." });
            }

            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == dto.ShowtimeId &&
                dto.SelectedSeatIds.Contains(h.SeatId) &&
                h.HoldExpiration > DateTime.Now).ToList();

            if (activeHolds.Any(h => h.ApplicationUserId != userId))
            {
                return Json(new { success = false, message = "Transaction failed: Another user is currently checking out with these seats." });
            }

            var userExistingHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == dto.ShowtimeId && h.ApplicationUserId == userId).ToList();

            if (userExistingHolds.Any())
            {
                _unitOfWork.SeatHold.RemoveRange(userExistingHolds);
            }

            var expirationTime = DateTime.Now.AddMinutes(10);

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

            _unitOfWork.Save();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult Checkout(int showtimeId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == showtimeId &&
                h.ApplicationUserId == userId &&
                h.HoldExpiration > DateTime.Now,
                includeProperties: "Seat").ToList();

            if (!activeHolds.Any())
            {
                TempData["error"] = "Your seat reservation has expired or no seats were selected. Please try again.";
                return RedirectToAction(nameof(SelectSeats), new { showtimeId = showtimeId });
            }

            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema"
            );

            if (showtime == null) return NotFound("Showtime details could not be loaded.");

            var vm = new CheckoutVM
            {
                Showtime = showtime,
                TicketItems = new List<TicketItemDto>()
            };

            decimal runningTotal = 0;

            foreach (var hold in activeHolds)
            {
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
        public IActionResult FinalizeOrder(int showtimeId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie"
            );

            if (showtime == null) return NotFound();

            decimal totalAmount = 0;
            foreach (var hold in activeHolds)
            {
                totalAmount += PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, hold.Seat.SeatType);
            }

            var newBooking = new Booking
            {
                ApplicationUserId = userId,
                ShowtimeId = showtimeId,
                Status = CinemaSystem.Models.Data.Enums.BookingStatus.Confirmed,
                PaymentStatus = CinemaSystem.Models.Data.Enums.PaymentStatus.Approved,
                TotalAmount = totalAmount,
                ConfirmationCode = "BK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                LoyaltyPointsEarned = (int)totalAmount,
                SessionId = "simulated_stripe_session_id",
                PaymentIntentId = "simulated_stripe_intent_id"
            };

            _unitOfWork.Booking.Add(newBooking);

            // OBJECTIVE FIX: Atomic ACID Transaction. We do not save yet.
            // By assigning the object reference (Booking = newBooking), EF Core wires the IDs automatically.
            foreach (var hold in activeHolds)
            {
                var ticket = new Ticket
                {
                    Booking = newBooking,
                    SeatId = hold.SeatId,
                    Price = PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, hold.Seat.SeatType),
                    Barcode = Guid.NewGuid().ToString()
                };
                _unitOfWork.Ticket.Add(ticket);
            }

            _unitOfWork.SeatHold.RemoveRange(activeHolds);

            // The database commits the parent booking, child tickets, and removes the holds in ONE atomic sweep.
            _unitOfWork.Save();

            return RedirectToAction(nameof(OrderConfirmation), new { bookingId = newBooking.Id });
        }

        [HttpGet]
        public IActionResult OrderConfirmation(int bookingId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = _unitOfWork.Booking.Get(
                b => b.Id == bookingId && b.ApplicationUserId == userId,
                includeProperties: "Showtime,Showtime.Movie,Showtime.CinemaHall,Showtime.CinemaHall.Cinema,Tickets,Tickets.Seat"
            );

            if (booking == null)
            {
                return NotFound("Order not found or access denied.");
            }

            return View(booking);
        }
        [HttpGet]
        [Authorize]
        public IActionResult DownloadTickets(int bookingId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Fetch the exact booking with all relational data
            var booking = _unitOfWork.Booking.Get(
                b => b.Id == bookingId && b.ApplicationUserId == userId,
                includeProperties: "Showtime,Showtime.Movie,Showtime.CinemaHall,Showtime.CinemaHall.Cinema,Tickets,Tickets.Seat"
            );

            if (booking == null) return NotFound("Order not found or access denied.");

            // 2. Build the PDF Document using QuestPDF Fluent API
            var document = Document.Create(container =>
            {
                // Format as a standard A4 page
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Text("CINEMA SYSTEM").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text($"Official Booking Receipt: {booking.ConfirmationCode}").FontSize(14).FontColor(Colors.Grey.Darken2);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // Body: Loop through each ticket and draw a distinct block
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(20);

                        foreach (var ticket in booking.Tickets)
                        {
                            col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten4).Padding(15).Row(row =>
                            {
                                // Left Column: Movie Info
                                row.RelativeItem().Column(ticketCol =>
                                {
                                    ticketCol.Item().Text(booking.Showtime.Movie.Title).FontSize(18).SemiBold();
                                    ticketCol.Item().Text($"Cinema: {booking.Showtime.CinemaHall.Cinema.Name} - Hall: {booking.Showtime.CinemaHall.Name}");
                                    ticketCol.Item().Text($"Date: {booking.Showtime.StartTime.ToString("dddd, MMM dd, yyyy - HH:mm")}");
                                });

                                // Right Column: Seat & Barcode Info
                                row.ConstantItem(150).AlignRight().Column(ticketCol =>
                                {
                                    ticketCol.Item().Text($"SEAT {ticket.Seat.Row}{ticket.Seat.Column}").FontSize(20).Bold().FontColor(Colors.Red.Medium);
                                    ticketCol.Item().Text($"Type: {ticket.Seat.SeatType}");
                                    ticketCol.Item().Text($"Price: ${ticket.Price.ToString("F2")}");
                                    ticketCol.Item().PaddingTop(10).Text($"ID: {ticket.Barcode.Substring(0, 8).ToUpper()}").FontSize(10).FontColor(Colors.Grey.Medium);
                                });
                            });
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("g"));
                        x.Span($" | Total Amount Paid: ${booking.TotalAmount.ToString("F2")}").SemiBold();
                    });
                });
            });

            // 3. Compile to byte array and stream to the browser
            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"CinemaTickets_{booking.ConfirmationCode}.pdf");
        }
    }
}