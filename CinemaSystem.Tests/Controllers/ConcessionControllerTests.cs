using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Hosting;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using CinemaSystem.Web.Areas.Admin.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.Tests.Controllers
{
    public class ConcessionControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnv;
        private readonly ConcessionController _controller;

        public ConcessionControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockWebHostEnv = new Mock<IWebHostEnvironment>();

            _mockUnitOfWork.Setup(u => u.Concession.Add(It.IsAny<Concession>()));
            _mockUnitOfWork.Setup(u => u.Concession.Update(It.IsAny<Concession>()));

            _controller = new ConcessionController(_mockUnitOfWork.Object, _mockWebHostEnv.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfConcessions()
        {
            var fakeList = new List<Concession> { new Concession { Id = 1, Name = "Popcorn" } };
            _mockUnitOfWork.Setup(u => u.Concession.GetAll(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeList);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Concession>>().Subject;
            model.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void UpsertGet_ReturnsViewResult_WithNewConcession_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Upsert(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<Concession>().Subject;
            model.Id.Should().Be(0);
        }

        [Fact]
        public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Concession)null);

            var result = _controller.Upsert(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UpsertGet_ReturnsViewResult_WithConcession_WhenEntityExists()
        {
            var existingConcession = new Concession { Id = 1, Name = "Nachos" };
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingConcession);

            var result = _controller.Upsert(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<Concession>().Subject;
            model.Name.Should().Be("Nachos");
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var concession = new Concession { Id = 0 };
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Upsert(concession, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(concession);

            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
        {
            var concession = new Concession { Id = 99, Name = "Ghost Item" };
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Concession)null);

            var result = _controller.Upsert(concession, null);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_AndSetsError_WhenFileTypeIsInvalid()
        {
            var concession = new Concession { Id = 0, Name = "Test" };
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("document.pdf");

            var result = _controller.Upsert(concession, mockFile.Object);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            _controller.TempData["error"].Should().Be("Invalid file type. Only JPG, PNG, and WEBP are allowed.");
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_AddsNewConcession_WhenAddModeAndNoFile()
        {
            var concession = new Concession { Id = 0, Name = "Large Cola", Price = 4.99m };

            var result = _controller.Upsert(concession, null);

            _mockUnitOfWork.Verify(u => u.Concession.Add(It.Is<Concession>(c => c.Name == "Large Cola" && c.Price == 4.99m)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Concession item created successfully.");
        }

        [Fact]
        public void UpsertPost_UpdatesConcession_WhenEditModeAndNoFile()
        {
            var existingConcession = new Concession { Id = 1, Name = "Old Name" };
            var concessionToUpdate = new Concession { Id = 1, Name = "New Name" };

            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingConcession);

            var result = _controller.Upsert(concessionToUpdate, null);

            _mockUnitOfWork.Verify(u => u.Concession.Update(It.Is<Concession>(c => c.Name == "New Name")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Concession item updated successfully.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Delete(id);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeleteGet_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Concession)null);

            var result = _controller.Delete(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeleteGet_ReturnsViewResult_WithEntity_WhenItExists()
        {
            var existingConcession = new Concession { Id = 1, Name = "Popcorn" };
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingConcession);

            var result = _controller.Delete(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(existingConcession);
        }

        [Fact]
        public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Concession)null);

            var result = _controller.DeletePOST(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()
        {
            var existingConcession = new Concession { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.Concession.Get(It.IsAny<Expression<Func<Concession, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingConcession);

            var result = _controller.DeletePOST(1);

            existingConcession.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.Concession.Update(existingConcession), Times.Once);
            _mockUnitOfWork.Verify(u => u.Concession.Remove(It.IsAny<Concession>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Concession item archived successfully.");
        }
    }
}