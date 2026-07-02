using CinemaSystem.DataAccess.Repository;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Web;
using CinemaSystem.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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
        private readonly Mock<IStringLocalizer<SharedResource>> _mockLocalizer;

        public ShowtimeControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();

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

            _controller = new ShowtimeController(_mockUnitOfWork.Object, _mockLocalizer.Object);
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


        [Fact]
        public void UpsertAjax_ReturnsSuccess_WhenDataIsValidAndShowtimeIsNew()
        {
            var today = DateTime.Now;
            var showtime = new Showtime
            {
                Id = 0,
                MovieId = 1,
                CinemaHallId = 1,
                StartTime = today.AddDays(1)
            };

            var movie = new Movie
            {
                Id = 1,
                DurationInMinutes = 100,
                StartDate = today.AddDays(-5),
                EndDate = today.AddDays(5)
            };

            _mockUnitOfWork.Setup(u => u.Movie.Get(
                It.IsAny<Expression<Func<Movie, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .Returns(movie);

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(
                It.IsAny<Expression<Func<Showtime, bool>>>(),
                It.IsAny<string>()))
                .Returns(new List<Showtime>());

            var result = _controller.UpsertAjax(showtime);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;

            jsonResult.Value.Should().BeEquivalentTo(new { success = true });

            _mockUnitOfWork.Verify(u => u.Showtime.Add(It.IsAny<Showtime>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }
    }
}