using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using CinemaSystem.Web.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;

namespace CinemaSystem.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking>());
            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime>());

            _controller = new DashboardController(_mockUnitOfWork.Object);
        }

        [Fact]
        public void Index_HandlesEmptyState_AndPreventsDivideByZero()
        {
            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<DashboardVM>().Subject;

            model.TotalRevenue.Should().Be(0);
            model.OccupancyRate.Should().Be(0);
            model.MovieLabels.Should().BeEmpty();
            model.DateLabels.Should().HaveCount(7);
            model.EmptyShowtimes.Should().BeEmpty();
        }

        [Fact]
        public void Index_SanitizesData_ByIgnoringSoftDeletedRecords()
        {
            var activeBooking = new Booking { Id = 1, TotalAmount = 100, IsDeleted = false, CreatedDate = DateTime.Now };
            var deletedBooking = new Booking { Id = 2, TotalAmount = 500, IsDeleted = true, CreatedDate = DateTime.Now };

            var dbBookings = new List<Booking> { activeBooking, deletedBooking };

            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns((Expression<Func<Booking, bool>> filter, string includeProperties) =>
                           {
                               if (filter != null)
                               {
                                   return dbBookings.AsQueryable().Where(filter).ToList();
                               }
                               return dbBookings;
                           });

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime>());

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<DashboardVM>().Subject;

            model.TotalRevenue.Should().Be(100);
        }

        [Fact]
        public void Index_CalculatesKPIs_AndOccupancyRateCorrectly()
        {
            var today = DateTime.Now.Date;

            var hall = new CinemaHall { TotalSeats = 100 };
            var show = new Showtime { Id = 1, StartTime = today.AddDays(-5), CinemaHall = hall, Movie = new Movie { Title = "A" } };

            var bookings = new List<Booking>
            {
                new Booking { ShowtimeId = 1, TotalAmount = 150, CreatedDate = today, Tickets = new List<Ticket> { new Ticket(), new Ticket() } },
                new Booking { ShowtimeId = 1, TotalAmount = 50, CreatedDate = today.AddDays(-1), Tickets = new List<Ticket> { new Ticket() } }
            };

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime> { show });
            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(bookings);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<DashboardVM>().Subject;

            model.TotalRevenue.Should().Be(200);
            model.TodayRevenue.Should().Be(150);
            model.TotalTicketsSold.Should().Be(3);

            model.OccupancyRate.Should().Be(3.0);
        }

        [Fact]
        public void Index_AggregatesCharts_ForTopMoviesAndRevenueTrend()
        {
            var today = DateTime.Now.Date;

            var showA = new Showtime { Id = 1, Movie = new Movie { Title = "Movie A" }, CinemaHall = new CinemaHall { TotalSeats = 50 } };
            var showB = new Showtime { Id = 2, Movie = new Movie { Title = "Movie B" }, CinemaHall = new CinemaHall { TotalSeats = 50 } };

            var bookings = new List<Booking>
            {
                new Booking { ShowtimeId = 1, CreatedDate = today, TotalAmount = 100, Tickets = new List<Ticket> { new Ticket(), new Ticket() } },
                new Booking { ShowtimeId = 2, CreatedDate = today, TotalAmount = 50, Tickets = new List<Ticket> { new Ticket() } },
                new Booking { ShowtimeId = 2, CreatedDate = today.AddDays(-1), TotalAmount = 50, Tickets = new List<Ticket> { new Ticket() } }
            };

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime> { showA, showB });
            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(bookings);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<DashboardVM>().Subject;

            model.MovieLabels.Should().Contain("Movie A");
            model.MovieLabels.Should().Contain("Movie B");
            model.MovieSalesData.Should().HaveCount(2);

            model.DateLabels.Should().HaveCount(7);
            model.DateLabels.Should().Contain(today.ToString("MMM dd"));

            var todayIndex = Array.IndexOf(model.DateLabels, today.ToString("MMM dd"));
            var yesterdayIndex = Array.IndexOf(model.DateLabels, today.AddDays(-1).ToString("MMM dd"));

            model.RevenueTrendData[todayIndex].Should().Be(150);
            model.RevenueTrendData[yesterdayIndex].Should().Be(50);
        }

        [Fact]
        public void Index_GeneratesAlerts_OnlyForEmptyShowtimesWithin24Hours()
        {
            var now = DateTime.Now;

            var showNext2HoursWithBooking = new Showtime { Id = 1, StartTime = now.AddHours(2), CinemaHall = new CinemaHall { Name = "H1" }, Movie = new Movie { Title = "Booked Movie" } };
            var showNext5HoursEmpty = new Showtime { Id = 2, StartTime = now.AddHours(5), CinemaHall = new CinemaHall { Name = "H2" }, Movie = new Movie { Title = "Empty Alert" } };
            var showNextWeekEmpty = new Showtime { Id = 3, StartTime = now.AddDays(3), CinemaHall = new CinemaHall { Name = "H3" }, Movie = new Movie { Title = "Too Far" } };

            var bookings = new List<Booking> { new Booking { ShowtimeId = 1, TotalAmount = 10 } };

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime> { showNext2HoursWithBooking, showNext5HoursEmpty, showNextWeekEmpty });
            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(bookings);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<DashboardVM>().Subject;

            model.EmptyShowtimes.Should().HaveCount(1);
            model.EmptyShowtimes.First().MovieTitle.Should().Be("Empty Alert");
        }
    }
}