using CinemaSystem.DataAccess.Repository;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace CinemaSystem.Tests.Controllers
{
    public class ShowtimeControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ShowtimeController _controller;

        public ShowtimeControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.CinemaHall.GetAll(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<CinemaHall>());
            _mockUnitOfWork.Setup(u => u.Movie.GetAll(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Movie>());
            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime>());

            var mockBookingRepo = new Mock<IBookingRepository>();
            mockBookingRepo.Setup(b => b.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking>());
            _mockUnitOfWork.SetupGet(u => u.Booking).Returns(mockBookingRepo.Object);

            _controller = new ShowtimeController(_mockUnitOfWork.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithCinemaHalls()
        {
            var fakeHalls = new List<CinemaHall> { new CinemaHall { Id = 1, Name = "Hall A" } };
            _mockUnitOfWork.Setup(u => u.CinemaHall.GetAll(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeHalls);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<CinemaHall>>().Subject;
            model.Should().HaveCount(1);
        }

        [Fact]
        public void UpsertGet_CalculatesCurrentWeek_WhenWeekStartIsNull()
        {
            var result = _controller.Upsert(1, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<ShowtimeCalendarVM>().Subject;

            model.CurrentWeekStart.DayOfWeek.Should().Be(DayOfWeek.Monday);
            model.CurrentWeekStart.Should().BeOnOrAfter(DateTime.Now.Date.AddDays(-7));
        }

        [Fact]
        public void UpsertGet_UsesSpecificDate_WhenWeekStartIsProvided()
        {
            var specificDate = new DateTime(2025, 1, 1);

            var result = _controller.Upsert(1, specificDate);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<ShowtimeCalendarVM>().Subject;

            model.CurrentWeekStart.Should().Be(specificDate.Date);
        }

        [Fact]
        public void UpsertAjax_ReturnsError_WhenPayloadIsNull()
        {
            var result = _controller.UpsertAjax(null);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "Invalid data payload." });
        }

        [Fact]
        public void UpsertAjax_ReturnsError_WhenMovieViolatesAvailabilityDates()
        {
            var showtime = new Showtime { MovieId = 1, StartTime = new DateTime(2025, 1, 1) };
            var movie = new Movie
            {
                Id = 1,
                StartDate = new DateTime(2025, 2, 1),
                EndDate = new DateTime(2025, 3, 1)
            };

            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(movie);

            var result = _controller.UpsertAjax(showtime);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new
            {
                success = false,
                message = $"Conflict: Movie is only available between {movie.StartDate.ToShortDateString()} and {movie.EndDate.ToShortDateString()}."
            });
        }

        [Fact]
        public void UpsertAjax_ReturnsError_WhenTimeOverlapOccurs()
        {
            var movie = new Movie { Id = 1, DurationInMinutes = 100, StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            var newShow = new Showtime { Id = 0, CinemaHallId = 1, MovieId = 1, StartTime = new DateTime(2025, 1, 1, 14, 0, 0) };

            var existingShow = new Showtime { CinemaHallId = 1, StartTime = new DateTime(2025, 1, 1, 12, 0, 0), EndTime = new DateTime(2025, 1, 1, 14, 10, 0) };

            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(movie);
            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime> { existingShow });

            var result = _controller.UpsertAjax(newShow);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new
            {
                success = false,
                message = "Time Conflict: There is another movie scheduled or the 20-minute cleaning buffer is being violated."
            });
        }

        [Fact]
        public void UpsertAjax_AddsShowtime_AndCalculatesEndTime_WhenValid()
        {
            var movie = new Movie { Id = 1, DurationInMinutes = 100, StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            var newShow = new Showtime { Id = 0, CinemaHallId = 1, MovieId = 1, StartTime = new DateTime(2025, 1, 1, 14, 0, 0) };

            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(movie);

            var result = _controller.UpsertAjax(newShow);

            _mockUnitOfWork.Verify(u => u.Showtime.Add(It.Is<Showtime>(s =>
                s.EndTime == new DateTime(2025, 1, 1, 16, 0, 0))), Times.Once);

            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new { success = true });
        }

        [Fact]
        public void UpsertAjax_BlocksEdit_WhenTicketsAreAlreadySold()
        {
            var movie = new Movie { Id = 1, StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            var updateRequest = new Showtime { Id = 1, MovieId = 2, StartTime = new DateTime(2025, 1, 1, 14, 0, 0) };
            var existingShow = new Showtime { Id = 1, MovieId = 1, StartTime = new DateTime(2025, 1, 1, 10, 0, 0) };

            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(movie);
            _mockUnitOfWork.Setup(u => u.Showtime.Get(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(existingShow);

            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking> { new Booking { Id = 1 } });

            var result = _controller.UpsertAjax(updateRequest);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new
            {
                success = false,
                message = "Operation Denied: Tickets have already been sold for this session. You cannot alter the time or movie."
            });
            _mockUnitOfWork.Verify(u => u.Showtime.Update(It.IsAny<Showtime>()), Times.Never);
        }

        [Fact]
        public void DeleteAjax_ReturnsError_WhenTicketsAreActive()
        {
            var existingShow = new Showtime { Id = 1 };
            _mockUnitOfWork.Setup(u => u.Showtime.Get(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(existingShow);

            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking> { new Booking { Id = 1, Status = BookingStatus.Confirmed } });

            var result = _controller.DeleteAjax(1);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new
            {
                success = false,
                message = "Cannot delete this showtime as active bookings (tickets) exist. Please cancel the bookings first."
            });
        }

        [Fact]
        public void DeleteAjax_SoftDeletesShowtime_WhenNoBookingsExist()
        {
            var existingShow = new Showtime { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.Showtime.Get(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(existingShow);

            var result = _controller.DeleteAjax(1);

            existingShow.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.Showtime.Update(existingShow), Times.Once);
            _mockUnitOfWork.Verify(u => u.Showtime.Remove(It.IsAny<Showtime>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new { success = true });
        }
    }
}