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
using System.Text.Json;
using CinemaSystem.Web.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Models.Data.Enums;

namespace CinemaSystem.Tests.Controllers
{
    public class CinemaHallControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CinemaHallController _controller;

        public CinemaHallControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.Cinema.GetAll(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Cinema>());
            _mockUnitOfWork.Setup(u => u.CinemaHall.Add(It.IsAny<CinemaHall>()));
            _mockUnitOfWork.Setup(u => u.CinemaHall.Update(It.IsAny<CinemaHall>()));
            _mockUnitOfWork.Setup(u => u.Seat.Add(It.IsAny<Seat>()));
            _mockUnitOfWork.Setup(u => u.Seat.RemoveRange(It.IsAny<IEnumerable<Seat>>()));

            _controller = new CinemaHallController(_mockUnitOfWork.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfCinemaHalls()
        {
            var fakeHalls = new List<CinemaHall> { new CinemaHall { Id = 1, Name = "Sala 1 IMAX" } };
            _mockUnitOfWork.Setup(u => u.CinemaHall.GetAll(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeHalls);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<CinemaHall>>().Subject;
            model.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void UpsertGet_ReturnsViewResult_WithCleanVM_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Upsert(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CinemaHallVM>().Subject;

            model.CinemaHall.Should().NotBeNull();
            model.CinemaHall.Id.Should().Be(0);
        }

        [Fact]
        public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.CinemaHall.Get(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((CinemaHall)null);

            var result = _controller.Upsert(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UpsertGet_ReturnsMappedVM_WithSerializedSeats_WhenEntityExists()
        {
            var existingHall = new CinemaHall
            {
                Id = 1,
                Name = "Sala VIP",
                Seats = new List<Seat>
                {
                    new Seat { Row = "A", Column = 1, SeatType = SeatType.Standard, IsAccessible = false },
                    new Seat { Row = "A", Column = 2, SeatType = SeatType.Premium, IsAccessible = true }
                }
            };

            _mockUnitOfWork.Setup(u => u.CinemaHall.Get(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingHall);

            var result = _controller.Upsert(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CinemaHallVM>().Subject;

            model.Rows.Should().Be(1);
            model.Cols.Should().Be(2);

            model.SeatLayoutData.Should().Contain("\"Row\":\"A\"");
            model.SeatLayoutData.Should().Contain("\"Type\":1");
        }


        [Fact]
        public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var vm = new CinemaHallVM { CinemaHall = new CinemaHall() };
            _controller.ModelState.AddModelError("CinemaHall.Name", "Name is required");

            var result = _controller.Upsert(vm);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(vm);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_WithTempDataError_WhenSeatLayoutDataIsMissing()
        {
            var vm = new CinemaHallVM { CinemaHall = new CinemaHall { Id = 0, Name = "Sala Test" }, SeatLayoutData = null };

            var result = _controller.Upsert(vm);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            _controller.TempData["error"].Should().Be("Seat layout could not be generated. Please try again.");
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_AddsNewHallAndSeats_WhenAddMode()
        {
            var incomingSeats = new List<SeatLayoutDto>
            {
                new SeatLayoutDto { Row = "A", Col = 1, Type = 0, IsAcc = false },
                new SeatLayoutDto { Row = "A", Col = 2, Type = 0, IsAcc = false }
            };

            var vm = new CinemaHallVM
            {
                CinemaHall = new CinemaHall { Id = 0, Name = "New Hall", CinemaId = 1 },
                SeatLayoutData = JsonSerializer.Serialize(incomingSeats),
                Rows = 1,
                Cols = 2
            };

            var result = _controller.Upsert(vm);

            _mockUnitOfWork.Verify(u => u.CinemaHall.Add(It.Is<CinemaHall>(h => h.Name == "New Hall" && h.TotalSeats == 2)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Seat.Add(It.IsAny<Seat>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.Save(), Times.Exactly(2));

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public void UpsertPost_BlocksUpdate_WhenShowtimesExistAndDimensionsChanged()
        {
            var existingHall = new CinemaHall { Id = 1, TotalSeats = 50, Seats = new List<Seat>() };
            _mockUnitOfWork.Setup(u => u.CinemaHall.Get(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingHall);

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime> { new Showtime { Id = 99 } });

            var incomingSeats = new List<SeatLayoutDto> { new SeatLayoutDto { Row = "A", Col = 1, Type = 0, IsAcc = false } };
            var vm = new CinemaHallVM
            {
                CinemaHall = new CinemaHall { Id = 1, TotalSeats = 100 },
                SeatLayoutData = JsonSerializer.Serialize(incomingSeats),
                Rows = 10,
                Cols = 10
            };

            var result = _controller.Upsert(vm);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Upsert");
            _controller.TempData["error"].Should().Be("Cannot change layout dimensions because showtimes are scheduled for this hall.");

            _mockUnitOfWork.Verify(u => u.CinemaHall.Update(It.IsAny<CinemaHall>()), Times.Never);
        }

        [Fact]
        public void UpsertPost_UpdatesSeats_AddsNew_RemovesOld_WhenEditModeIsValid()
        {
            var oldSeatToKeep = new Seat { Row = "A", Column = 1, SeatType = SeatType.Standard };
            var oldSeatToRemove = new Seat { Row = "A", Column = 2, SeatType = SeatType.Standard };

            var existingHall = new CinemaHall
            {
                Id = 1,
                Name = "Old Hall",
                TotalSeats = 2,
                Seats = new List<Seat> { oldSeatToKeep, oldSeatToRemove }
            };

            _mockUnitOfWork.Setup(u => u.CinemaHall.Get(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingHall);
            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<Showtime>());

            var incomingSeats = new List<SeatLayoutDto>
            {
                new SeatLayoutDto { Row = "A", Col = 1, Type = (int)SeatType.Premium, IsAcc = true },
                new SeatLayoutDto { Row = "B", Col = 1, Type = (int)SeatType.Standard, IsAcc = false }
            };

            var vm = new CinemaHallVM
            {
                CinemaHall = new CinemaHall { Id = 1, Name = "Renamed Hall", TotalSeats = 2 },
                SeatLayoutData = JsonSerializer.Serialize(incomingSeats),
                Rows = 2,
                Cols = 1
            };

            var result = _controller.Upsert(vm);

            oldSeatToKeep.SeatType.Should().Be(SeatType.Premium);

            _mockUnitOfWork.Verify(u => u.Seat.Add(It.Is<Seat>(s => s.Row == "B" && s.Column == 1)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Seat.RemoveRange(It.Is<IEnumerable<Seat>>(list => list.Contains(oldSeatToRemove))), Times.Once);

            _mockUnitOfWork.Verify(u => u.CinemaHall.Update(It.Is<CinemaHall>(h => h.Name == "Renamed Hall")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }


        [Fact]
        public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero()
        {
            _controller.Delete(0).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()
        {
            var existingHall = new CinemaHall { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.CinemaHall.Get(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingHall);

            var result = _controller.DeletePOST(1);

            existingHall.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.CinemaHall.Update(existingHall), Times.Once);
            _mockUnitOfWork.Verify(u => u.CinemaHall.Remove(It.IsAny<CinemaHall>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }
    }
}