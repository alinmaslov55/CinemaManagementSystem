using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CinemaSystem.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ITicketPdfService _ticketPdfService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BookingController(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ITicketPdfService ticketPdfService,
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<SharedResource> localizer) // Injectare
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _ticketPdfService = ticketPdfService;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult SelectSeats(int showtimeId)
        {
            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema,CinemaHall.Seats"
            );

            if (showtime == null) return NotFound(_localizer["Error_ShowtimeNotFound"].Value);

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
        [ValidateAntiForgeryToken]
        public IActionResult LockSeatsAjax([FromBody] HoldSeatsRequestDto dto)
        {
            if (dto == null || dto.SelectedSeatIds == null || !dto.SelectedSeatIds.Any())
            {
                return Json(new { success = false, message = _localizer["Error_NoSeatsSelected"].Value });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookedSeats = _unitOfWork.Ticket?.GetAll(t =>
                t.Booking.ShowtimeId == dto.ShowtimeId &&
                dto.SelectedSeatIds.Contains(t.SeatId)).ToList() ?? new List<Ticket>();

            if (bookedSeats.Any())
            {
                return Json(new { success = false, message = _localizer["Error_SeatsAlreadyPurchased"].Value });
            }

            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == dto.ShowtimeId &&
                dto.SelectedSeatIds.Contains(h.SeatId) &&
                h.HoldExpiration > DateTime.Now).ToList();

            if (activeHolds.Any(h => h.ApplicationUserId != userId))
            {
                return Json(new { success = false, message = _localizer["Error_CheckoutCollision"].Value });
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
                TempData["error"] = _localizer["Error_SeatHoldExpired"].Value;
                return RedirectToAction(nameof(SelectSeats), new { showtimeId = showtimeId });
            }

            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema"
            );

            if (showtime == null) return NotFound(_localizer["Error_ShowtimeDetailsFailed"].Value);

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
            vm.AvailableFnBProducts = _unitOfWork.FnBProduct.GetAll(c => c.IsActive).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizeOrder(int showtimeId, int[] FnBProductIds, int[] FnBProductQuantities)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) return Unauthorized();

            var activeHolds = _unitOfWork.SeatHold.GetAll(h =>
                h.ShowtimeId == showtimeId &&
                h.ApplicationUserId == userId &&
                h.HoldExpiration > DateTime.Now,
                includeProperties: "Seat").ToList();

            if (!activeHolds.Any())
            {
                TempData["error"] = _localizer["Error_SessionExpired"].Value;
                return RedirectToAction(nameof(SelectSeats), new { showtimeId = showtimeId });
            }

            var showtime = _unitOfWork.Showtime.Get(
                s => s.Id == showtimeId,
                includeProperties: "Movie"
            );

            if (showtime == null) return NotFound();

            decimal ticketsTotal = 0;
            foreach (var hold in activeHolds)
            {
                ticketsTotal += PricingCalculator.CalculateSeatPrice(showtime.Movie.Price, showtime.Price, hold.Seat.SeatType);
            }

            var newBooking = new Booking
            {
                ApplicationUserId = userId,
                ShowtimeId = showtimeId,
                Status = CinemaSystem.Models.Data.Enums.BookingStatus.Confirmed,
                PaymentStatus = CinemaSystem.Models.Data.Enums.PaymentStatus.Approved,
                ConfirmationCode = "BK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                SessionId = "simulated_stripe_session_id",
                PaymentIntentId = "simulated_stripe_intent_id"
            };

            decimal FnBProductsTotal = 0;

            if (FnBProductIds != null && FnBProductQuantities != null && FnBProductIds.Length == FnBProductQuantities.Length)
            {
                for (int i = 0; i < FnBProductIds.Length; i++)
                {
                    if (FnBProductQuantities[i] > 0)
                    {
                        var concession = _unitOfWork.FnBProduct.Get(c => c.Id == FnBProductIds[i]);
                        if (concession != null)
                        {
                            FnBProductsTotal += (concession.Price * FnBProductQuantities[i]);

                            newBooking.BookingFnBs.Add(new BookingFnB
                            {
                                FnBProductId = concession.Id,
                                Quantity = FnBProductQuantities[i],
                                PriceAtPurchase = concession.Price
                            });
                        }
                    }
                }
            }

            decimal subTotal = ticketsTotal + FnBProductsTotal;
            decimal discountPercentage = 0m;

            if (currentUser.MembershipTier == "Gold") discountPercentage = 0.15m;
            else if (currentUser.MembershipTier == "Silver") discountPercentage = 0.10m;

            decimal discountAmount = subTotal * discountPercentage;
            newBooking.TotalAmount = subTotal - discountAmount;

            newBooking.LoyaltyPointsEarned = (int)Math.Round(newBooking.TotalAmount);
            currentUser.LoyaltyPoints += newBooking.LoyaltyPointsEarned;
            await _userManager.UpdateAsync(currentUser);

            _unitOfWork.Booking.Add(newBooking);

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
            _unitOfWork.Save();

            var completedBooking = _unitOfWork.Booking.Get(
                b => b.Id == newBooking.Id,
                includeProperties: "Showtime,Showtime.Movie,Showtime.CinemaHall,Showtime.CinemaHall.Cinema,Tickets,Tickets.Seat,User"
            );

            if (completedBooking != null && !string.IsNullOrEmpty(completedBooking.User?.Email))
            {
                byte[] pdfAttachment = _ticketPdfService.GenerateTicketPdfBytes(completedBooking);

                // Email formatat localizat (Title și Code sunt injectate în cheia din dicționar)
                string subject = string.Format(_localizer["Email_TicketSubject"].Value, completedBooking.Showtime.Movie.Title, completedBooking.ConfirmationCode);
                string htmlBody = _localizer["Email_TicketBody"].Value;

                try
                {
                    _emailService.SendEmailWithAttachmentAsync(completedBooking.User.Email, subject, htmlBody, pdfAttachment, $"Order_{completedBooking.ConfirmationCode}.pdf").GetAwaiter().GetResult();
                }
                catch
                {
                }
            }

            return RedirectToAction(nameof(OrderConfirmation), new { bookingId = newBooking.Id });
        }

        [HttpGet]
        public IActionResult OrderConfirmation(int bookingId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = _unitOfWork.Booking.Get(
                b => b.Id == bookingId && b.ApplicationUserId == userId,
                includeProperties: "Showtime,Showtime.Movie,Showtime.CinemaHall,Showtime.CinemaHall.Cinema,Tickets,Tickets.Seat,BookingFnBs,BookingFnBs.FnBProduct"
            );

            if (booking == null) return NotFound(_localizer["Error_OrderNotFound"].Value);

            return View(booking);
        }

        [HttpGet]
        public IActionResult DownloadTickets(int bookingId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = _unitOfWork.Booking.Get(
                b => b.Id == bookingId && b.ApplicationUserId == userId,
                includeProperties: "Showtime,Showtime.Movie,Showtime.CinemaHall,Showtime.CinemaHall.Cinema,Tickets,Tickets.Seat,BookingFnBs,BookingFnBs.FnBProduct"
            );

            if (booking == null) return NotFound(_localizer["Error_OrderNotFound"].Value);

            byte[] pdfBytes = _ticketPdfService.GenerateTicketPdfBytes(booking);

            return File(pdfBytes, "application/pdf", $"CinemaTickets_{booking.ConfirmationCode}.pdf");
        }

        [HttpGet]
        public IActionResult History()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = _unitOfWork.Booking.GetAll(
                b => b.ApplicationUserId == userId,
                includeProperties: "Showtime,Showtime.Movie,Tickets"
            ).OrderByDescending(b => b.Id).ToList();

            return View(bookings);
        }
    }
}