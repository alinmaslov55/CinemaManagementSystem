using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Web;
using CinemaSystem.Web.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace CinemaSystem.Tests.Controllers
{
    public class FnBProductControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnv;
        private readonly Mock<IStringLocalizer<SharedResource>> _mockLocalizer;

        private readonly FnBProductController _controller;

        public FnBProductControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockWebHostEnv = new Mock<IWebHostEnvironment>();

            _mockUnitOfWork.Setup(u => u.FnBProduct.Add(It.IsAny<FnBProduct>()));
            _mockUnitOfWork.Setup(u => u.FnBProduct.Update(It.IsAny<FnBProduct>()));

            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();
            _controller = new FnBProductController(_mockUnitOfWork.Object, _mockWebHostEnv.Object, _mockLocalizer.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfFoodAndBeverages()
        {
            var fakeList = new List<FnBProduct> { new FnBProduct { Id = 1, Name = "Popcorn" } };
            _mockUnitOfWork.Setup(u => u.FnBProduct.GetAll(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeList);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<FnBProduct>>().Subject;
            model.Should().HaveCount(1);
        }

        [Fact]
        public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()
        {
            var existingFnBProduct = new FnBProduct { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingFnBProduct);

            _mockLocalizer.Setup(l => l["FnB_ArchivedSuccess"])
                          .Returns(new LocalizedString("FnB_ArchivedSuccess", "Food & Beverage item archived successfully."));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = _controller.DeletePOST(1);

            existingFnBProduct.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.FnBProduct.Update(existingFnBProduct), Times.Once);
            _mockUnitOfWork.Verify(u => u.FnBProduct.Remove(It.IsAny<FnBProduct>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");

            _controller.TempData["success"].Should().Be("Food & Beverage item archived successfully.");
        }

        [Fact]
        public void UpsertPost_AddsNewFnBProduct_WhenAddModeAndNoFile()
        {
            var newFnBProduct = new FnBProduct { Id = 0, Name = "Large Cola", Price = 4.99m };

            _mockLocalizer.Setup(l => l["FnB_CreatedSuccess"])
                          .Returns(new LocalizedString("FnB_CreatedSuccess", "Food & Beverage item created successfully."));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockWebHostEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            var result = _controller.Upsert(newFnBProduct, null);

            _mockUnitOfWork.Verify(u => u.FnBProduct.Add(It.Is<FnBProduct>(c => c.Name == "Large Cola" && c.Price == 4.99m)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");

            _controller.TempData["success"].Should().Be("Food & Beverage item created successfully.");
        }

        [Fact]
        public void UpsertPost_UpdatesFnBProduct_WhenEditModeAndNoFile()
        {
            var existingFnBProduct = new FnBProduct { Id = 1, Name = "Old Name" };
            var productToUpdate = new FnBProduct { Id = 1, Name = "New Name" };

            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingFnBProduct);

            _mockLocalizer.Setup(l => l["FnB_UpdatedSuccess"])
                          .Returns(new LocalizedString("FnB_UpdatedSuccess", "Food & Beverage item updated successfully."));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockWebHostEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            var result = _controller.Upsert(productToUpdate, null);

            _mockUnitOfWork.Verify(u => u.FnBProduct.Update(It.Is<FnBProduct>(c => c.Name == "New Name")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");

            _controller.TempData["success"].Should().Be("Food & Beverage item updated successfully.");
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_AndSetsError_WhenFileTypeIsInvalid()
        {
            var concession = new FnBProduct { Id = 0, Name = "Test" };
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("document.pdf");

            _mockLocalizer.Setup(l => l["FnB_InvalidFileType"])
                          .Returns(new LocalizedString("FnB_InvalidFileType", "Invalid file type. Only JPG, PNG, and WEBP are allowed."));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockWebHostEnv.Setup(m => m.WebRootPath).Returns("wwwroot");

            var result = _controller.Upsert(concession, mockFile.Object);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            _controller.TempData["error"].Should().Be("Invalid file type. Only JPG, PNG, and WEBP are allowed.");
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void UpsertGet_ReturnsViewResult_WithNewConcession_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Upsert(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<FnBProduct>().Subject;
            model.Id.Should().Be(0);
        }

        [Fact]
        public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((FnBProduct)null);

            var result = _controller.Upsert(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UpsertGet_ReturnsViewResult_WithConcession_WhenEntityExists()
        {
            var existingConcession = new FnBProduct { Id = 1, Name = "Nachos" };
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingConcession);

            var result = _controller.Upsert(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<FnBProduct>().Subject;
            model.Name.Should().Be("Nachos");
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var concession = new FnBProduct { Id = 0 };
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Upsert(concession, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(concession);

            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
        {
            var concession = new FnBProduct { Id = 99, Name = "Ghost Item" };
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((FnBProduct)null);

            var result = _controller.Upsert(concession, null);

            result.Should().BeOfType<NotFoundResult>();
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
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((FnBProduct)null);

            var result = _controller.Delete(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeleteGet_ReturnsViewResult_WithEntity_WhenItExists()
        {
            var existingConcession = new FnBProduct { Id = 1, Name = "Popcorn" };
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingConcession);

            var result = _controller.Delete(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(existingConcession);
        }

        [Fact]
        public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.FnBProduct.Get(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((FnBProduct)null);

            var result = _controller.DeletePOST(99);

            result.Should().BeOfType<NotFoundResult>();
        }


    }
}