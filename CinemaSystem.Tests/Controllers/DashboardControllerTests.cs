using CinemaSystem.DataAccess.Repository.IRepository;
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
    public class DashboardControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DashboardController _controller;
        private readonly Mock<IStringLocalizer<SharedResource>> _mockLocalizer;


        public DashboardControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking>());
            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime>());

            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();
            _controller = new DashboardController(_mockUnitOfWork.Object, _mockLocalizer.Object);
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
        public void Index_CalculatesOccupancyRateAsZero_WhenCapacityIsZero()
        {
            var show = new Showtime { Id = 1, CinemaHall = new CinemaHall { TotalSeats = 0 }, Movie = new Movie { Title = "Test" } };

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime> { show });
            _mockUnitOfWork.Setup(u => u.Booking.GetAll(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Booking>());

            var result = _controller.Index();

            var model = ((ViewResult)result).Model.Should().BeOfType<DashboardVM>().Subject;
            model.OccupancyRate.Should().Be(0);
        }
    }
}