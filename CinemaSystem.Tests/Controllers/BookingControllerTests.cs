using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CinemaSystem.Web.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Utility;

namespace CinemaSystem.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ITicketPdfService> _mockTicketPdfService;
        private readonly BookingController _controller;
        private readonly string _testUserId = "user-123";

        public BookingControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEmailService = new Mock<IEmailService>();
            _mockTicketPdfService = new Mock<ITicketPdfService>();

            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking>());
            _mockUnitOfWork.Setup(u => u.Ticket.GetAll(It.IsAny<Expression<Func<Ticket, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Ticket>());
            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<SeatHold>());
            _mockUnitOfWork.Setup(u => u.Concession.GetAll(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Concession>());

            _mockTicketPdfService.Setup(p => p.GenerateTicketPdfBytes(It.IsAny<Booking>()))
                                 .Returns(new byte[] { 1, 2, 3 });

            _controller = new BookingController(_mockUnitOfWork.Object, _mockEmailService.Object, _mockTicketPdfService.Object);
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
                new Claim(ClaimTypes.Name, "test@user.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public void SelectSeats_ReturnsNotFound_WhenShowtimeIsInvalid()
        {
            _mockUnitOfWork.Setup(u => u.Showtime.Get(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns((Showtime)null);

            var result = _controller.SelectSeats(99);

            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Showtime not found.");
        }

        [Fact]
        public void SelectSeats_MapsSeatStatusesCorrectly()
        {
            var availableSeat = new Seat { Id = 1, Row = "A", Column = 1, SeatType = SeatType.Standard };
            var bookedSeat = new Seat { Id = 2, Row = "A", Column = 2, SeatType = SeatType.Standard };
            var heldSeat = new Seat { Id = 3, Row = "A", Column = 3, SeatType = SeatType.Premium };

            var showtime = new Showtime
            {
                Id = 1,
                Movie = new Movie { Price = 10 },
                Price = 2,
                CinemaHall = new CinemaHall { Seats = new List<Seat> { availableSeat, bookedSeat, heldSeat } }
            };

            var tickets = new List<Ticket> { new Ticket { SeatId = 2, Booking = new Booking { ShowtimeId = 1 } } };
            var holds = new List<SeatHold> { new SeatHold { SeatId = 3, ShowtimeId = 1, HoldExpiration = DateTime.Now.AddMinutes(5) } };

            _mockUnitOfWork.Setup(u => u.Showtime.Get(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(showtime);
            _mockUnitOfWork.Setup(u => u.Ticket.GetAll(It.IsAny<Expression<Func<Ticket, bool>>>(), It.IsAny<string>())).Returns(tickets);
            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>())).Returns(holds);

            var result = _controller.SelectSeats(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var vm = viewResult.Model.Should().BeOfType<SeatSelectionVM>().Subject;

            vm.Seats.Should().HaveCount(3);
            vm.Seats.Single(s => s.Id == 1).Status.Should().Be("Available");
            vm.Seats.Single(s => s.Id == 2).Status.Should().Be("Booked");
            vm.Seats.Single(s => s.Id == 3).Status.Should().Be("Held");
        }

        [Fact]
        public void LockSeatsAjax_Fails_WhenSeatIsAlreadyBooked()
        {
            var dto = new HoldSeatsRequestDto { ShowtimeId = 1, SelectedSeatIds = new List<int> { 5 } };

            var existingTickets = new List<Ticket> { new Ticket { SeatId = 5, Booking = new Booking { ShowtimeId = 1 } } };
            _mockUnitOfWork.Setup(u => u.Ticket.GetAll(It.IsAny<Expression<Func<Ticket, bool>>>(), It.IsAny<string>())).Returns(existingTickets);

            var result = _controller.LockSeatsAjax(dto);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "Transaction failed: One or more seats have already been purchased." });
        }

        [Fact]
        public void LockSeatsAjax_CreatesNewHolds_AndClearsOldOnes()
        {
            var dto = new HoldSeatsRequestDto { ShowtimeId = 1, SelectedSeatIds = new List<int> { 1, 2 } };

            var existingUserHolds = new List<SeatHold> { new SeatHold { Id = 99, ApplicationUserId = _testUserId, ShowtimeId = 1 } };
            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>())).Returns(existingUserHolds);

            _mockUnitOfWork.Setup(u => u.SeatHold.RemoveRange(It.IsAny<IEnumerable<SeatHold>>())).Verifiable();
            _mockUnitOfWork.Setup(u => u.SeatHold.Add(It.IsAny<SeatHold>())).Verifiable();

            var result = _controller.LockSeatsAjax(dto);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new { success = true });

            _mockUnitOfWork.Verify(u => u.SeatHold.RemoveRange(existingUserHolds), Times.Once);
            _mockUnitOfWork.Verify(u => u.SeatHold.Add(It.Is<SeatHold>(h => h.SeatId == 1 && h.ApplicationUserId == _testUserId)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SeatHold.Add(It.Is<SeatHold>(h => h.SeatId == 2 && h.ApplicationUserId == _testUserId)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void Checkout_RedirectsToSelect_WhenHoldsExpired()
        {
            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>())).Returns(new List<SeatHold>());

            var result = _controller.Checkout(1);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("SelectSeats");
            _controller.TempData["error"].Should().Be("Your seat reservation has expired or no seats were selected. Please try again.");
        }

        [Fact]
        public void FinalizeOrder_ProcessesSuccessfully_WithEmailAndTickets()
        {
            int showtimeId = 1;
            var mockSeat = new Seat { Id = 10, SeatType = SeatType.Standard, Row = "A", Column = 1 };

            var activeHolds = new List<SeatHold> { new SeatHold { SeatId = 10, Seat = mockSeat, ApplicationUserId = _testUserId, HoldExpiration = DateTime.Now.AddMinutes(5) } };

            var showtime = new Showtime { Id = showtimeId, Movie = new Movie { Title = "Test Movie", Price = 10 }, Price = 0 };

            var concession = new Concession { Id = 5, Price = 5.0m };
            int[] cIds = { 5 };
            int[] cQtys = { 2 };

            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>())).Returns(activeHolds);
            _mockUnitOfWork.Setup(u => u.Showtime.Get(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(showtime);
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(concession);

            _mockUnitOfWork.Setup(u => u.Booking.Get(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Booking { Id = 100, ConfirmationCode = "TEST-123", User = new ApplicationUser { Email = "user@test.com" }, Showtime = new Showtime { Movie = new Movie { Title = "X" }, CinemaHall = new CinemaHall { Cinema = new Cinema() } }, Tickets = new List<Ticket>() });

            var result = _controller.FinalizeOrder(showtimeId, cIds, cQtys);

            _mockUnitOfWork.Verify(u => u.Booking.Add(It.Is<Booking>(b => b.ApplicationUserId == _testUserId)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Ticket.Add(It.Is<Ticket>(t => t.SeatId == 10)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SeatHold.RemoveRange(activeHolds), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            _mockTicketPdfService.Verify(p => p.GenerateTicketPdfBytes(It.IsAny<Booking>()), Times.Once);
            _mockEmailService.Verify(e => e.SendEmailWithAttachmentAsync("user@test.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("OrderConfirmation");
        }

        [Fact]
        public void DownloadTickets_ReturnsPdfFile_WhenBookingIsValid()
        {
            var mockBooking = new Booking { Id = 1, ApplicationUserId = _testUserId, ConfirmationCode = "MOCK-XYZ" };

            _mockUnitOfWork.Setup(u => u.Booking.Get(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(mockBooking);

            var fakePdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            _mockTicketPdfService.Setup(p => p.GenerateTicketPdfBytes(mockBooking)).Returns(fakePdfBytes);

            var result = _controller.DownloadTickets(1);

            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.ContentType.Should().Be("application/pdf");
            fileResult.FileDownloadName.Should().Be("CinemaTickets_MOCK-XYZ.pdf");
            fileResult.FileContents.Should().BeEquivalentTo(fakePdfBytes);

            _mockTicketPdfService.Verify(p => p.GenerateTicketPdfBytes(mockBooking), Times.Once);
        }

        [Fact]
        public void History_FiltersStrictlyByUserIdentity()
        {
            var result = _controller.History();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            _mockUnitOfWork.Verify(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()), Times.Once);
        }
    }
}