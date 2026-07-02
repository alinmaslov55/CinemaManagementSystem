using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Web;
using CinemaSystem.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Linq.Expressions;
using Xunit;

namespace CinemaSystem.Tests.Controllers
{
    public partial class CinemaControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnv;
        private readonly CinemaController _controller;
        private readonly Mock<IStringLocalizer<SharedResource>> _mockLocalizer;

        public CinemaControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockWebHostEnv = new Mock<IWebHostEnvironment>();
            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();

            _controller = new CinemaController(_mockUnitOfWork.Object, _mockWebHostEnv.Object, _mockLocalizer.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfCinemas()
        {
            var fakeCinemaList = new List<Cinema>
            {
                new Cinema { Id = 1, Name = "Cinema Timisoara", City = "Timisoara", Address = "Str. Alba" },
                new Cinema { Id = 2, Name = "Cinema Bucuresti", City = "Bucuresti", Address = "Blv. Unirii" }
            };

            _mockUnitOfWork.Setup(u => u.Cinema.GetAll(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeCinemaList);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Cinema>>().Subject;

            model.Should().HaveCount(2);
            model.First().Name.Should().Be("Cinema Timisoara");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void UpsertGet_ReturnsViewResult_WithNewCinemaVM_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Upsert(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CinemaVM>().Subject;

            model.Id.Should().Be(0);
            model.Name.Should().BeNull();
        }

        [Fact]
        public void UpsertGet_ReturnsNotFound_WhenIdIsValidButEntityDoesNotExist()
        {
            int searchId = 99;

            _mockUnitOfWork.Setup(u => u.Cinema.Get(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Cinema)null);

            var result = _controller.Upsert(searchId);

            result.Should().BeOfType<NotFoundResult>("the database returned null for the provided ID");
        }

        [Fact]
        public void UpsertGet_ReturnsViewResult_WithMappedViewModel_WhenEntityExists()
        {
            int searchId = 1;
            var existingCinema = new Cinema
            {
                Id = searchId,
                Name = "IMAX Center",
                Description = "3D Experience",
                City = "Cluj",
                Address = "Iulius Mall",
                Logo = "/images/imax.jpg"
            };

            _mockUnitOfWork.Setup(u => u.Cinema.Get(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingCinema);

            var result = _controller.Upsert(searchId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CinemaVM>().Subject;

            model.Id.Should().Be(existingCinema.Id);
            model.Name.Should().Be(existingCinema.Name);
            model.Description.Should().Be(existingCinema.Description);
            model.City.Should().Be(existingCinema.City);
            model.Address.Should().Be(existingCinema.Address);
            model.Logo.Should().Be(existingCinema.Logo);
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var vm = new CinemaVM { Id = 0 };
            _controller.ModelState.AddModelError("Name", "Cinema Name is required");

            var result = _controller.Upsert(vm, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(vm);

            _mockUnitOfWork.Verify(u => u.Cinema.Add(It.IsAny<Cinema>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Cinema.Update(It.IsAny<Cinema>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
        {
            var vm = new CinemaVM { Id = 99, Name = "Ghost Cinema", City = "Test", Address = "Test" };

            _mockUnitOfWork.Setup(u => u.Cinema.Get(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Cinema)null);

            var result = _controller.Upsert(vm, null);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Cinema.Get(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Cinema)null);

            var result = _controller.DeletePOST(99);

            result.Should().BeOfType<NotFoundResult>();
        }
        public void DeletePOST_SetsIsDeletedToTrue_AndCallsUpdate_WhenEntityExists()
        {
            var existingCinema = new Cinema { Id = 1, IsDeleted = false };

            _mockUnitOfWork.Setup(u => u.Cinema.Get(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingCinema);

            _mockLocalizer.Setup(l => l["Cinema_ArchivedSuccess"])
                          .Returns(new LocalizedString("Cinema_ArchivedSuccess", "Cinema archived successfully"));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = _controller.DeletePOST(1);

            existingCinema.IsDeleted.Should().BeTrue();

            _mockUnitOfWork.Verify(u => u.Cinema.Update(existingCinema), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
            _mockUnitOfWork.Verify(u => u.Cinema.Remove(It.IsAny<Cinema>()), Times.Never);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Cinema archived successfully");
        }

        public void UpsertPost_AddsNewCinema_WhenAddModeAndNoFile()
        {
            var vm = new CinemaVM
            {
                Id = 0,
                Name = "New Cinema",
                City = "New City",
                Address = "New Address"
            };

            _mockLocalizer.Setup(l => l["Cinema_CreatedSuccess"])
                          .Returns(new LocalizedString("Cinema_CreatedSuccess", "Cinema created successfully"));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockWebHostEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            _mockUnitOfWork.Setup(u => u.Cinema.Add(It.IsAny<Cinema>()));

            var result = _controller.Upsert(vm, null);

            _mockUnitOfWork.Verify(u => u.Cinema.Add(It.Is<Cinema>(c =>
                c.Name == "New Cinema" &&
                c.City == "New City" &&
                c.Address == "New Address")), Times.Once);

            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Cinema created successfully");
        }

        public void UpsertPost_ReturnsViewResult_AndSetsTempDataError_WhenFileTypeIsInvalid()
        {
            var vm = new CinemaVM { Id = 0, Name = "Test Cinema", City = "Test", Address = "Test" };

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("malware.exe");

            _mockLocalizer.Setup(l => l["Cinema_InvalidFileType"])
                          .Returns(new LocalizedString("Cinema_InvalidFileType", "Invalid file type. Only JPG, PNG, and WEBP are allowed."));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockWebHostEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            var result = _controller.Upsert(vm, mockFile.Object);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            _controller.TempData["error"].Should().Be("Invalid file type. Only JPG, PNG, and WEBP are allowed.");

            _mockUnitOfWork.Verify(u => u.Cinema.Add(It.IsAny<Cinema>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Cinema.Update(It.IsAny<Cinema>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        public void UpsertPost_UpdatesCinema_WhenEditModeAndNoFile()
        {
            var vm = new CinemaVM
            {
                Id = 1,
                Name = "Updated Cinema",
                City = "Updated City",
                Address = "Updated Address"
            };

            var existingCinema = new Cinema
            {
                Id = 1,
                Name = "Old Name",
                City = "Old City",
                Address = "Old Address"
            };

            _mockUnitOfWork.Setup(u => u.Cinema.Get(It.IsAny<Expression<Func<Cinema, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingCinema);

            _mockLocalizer.Setup(l => l["Cinema_UpdatedSuccess"])
                          .Returns(new LocalizedString("Cinema_UpdatedSuccess", "Cinema updated successfully"));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockWebHostEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            var result = _controller.Upsert(vm, null);

            _mockUnitOfWork.Verify(u => u.Cinema.Update(It.Is<Cinema>(c =>
                c.Id == 1 &&
                c.Name == "Updated Cinema" &&
                c.City == "Updated City" &&
                c.Address == "Updated Address")), Times.Once);

            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Cinema updated successfully");
        }
    }
}