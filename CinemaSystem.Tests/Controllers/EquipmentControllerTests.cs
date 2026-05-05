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
using CinemaSystem.Web.Areas.Admin.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;

namespace CinemaSystem.Tests.Controllers
{
    public class EquipmentControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly EquipmentController _controller;

        public EquipmentControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.CinemaHall.GetAll(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<CinemaHall>());
            _mockUnitOfWork.Setup(u => u.Equipment.Add(It.IsAny<Equipment>()));
            _mockUnitOfWork.Setup(u => u.Equipment.Update(It.IsAny<Equipment>()));

            _controller = new EquipmentController(_mockUnitOfWork.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfEquipment()
        {
            var fakeList = new List<Equipment> { new Equipment { Id = 1, Name = "4K Projector" } };
            _mockUnitOfWork.Setup(u => u.Equipment.GetAll(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeList);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Equipment>>().Subject;
            model.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void UpsertGet_ReturnsViewResult_WithNewEquipment_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Upsert(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<EquipmentVM>().Subject;

            model.Equipment.Id.Should().Be(0);

            model.Equipment.PurchaseDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void UpsertGet_RedirectsToIndex_AndSetsTempData_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Equipment)null);

            var result = _controller.Upsert(99);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["error"].Should().Be("Error: Asset could not be found.");
        }

        [Fact]
        public void UpsertGet_ReturnsViewResult_WithEquipmentVM_WhenEntityExists()
        {
            var existingEquipment = new Equipment { Id = 1, Name = "Surround Sound System" };
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingEquipment);

            var result = _controller.Upsert(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<EquipmentVM>().Subject;
            model.Equipment.Name.Should().Be("Surround Sound System");
        }

        [Fact]
        public void UpsertPost_ReturnsViewResult_AndRepopulatesDropdown_WhenModelStateIsInvalid()
        {
            var vm = new EquipmentVM { Equipment = new Equipment { Id = 0 } };
            _controller.ModelState.AddModelError("Equipment.Name", "Name is required");

            var fakeHalls = new List<CinemaHall> { new CinemaHall { Id = 1, Name = "Hall A", Cinema = new Cinema { Name = "Cinema 1" } } };
            _mockUnitOfWork.Setup(u => u.CinemaHall.GetAll(It.IsAny<Expression<Func<CinemaHall, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeHalls);

            var result = _controller.Upsert(vm);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var returnedModel = viewResult.Model.Should().BeOfType<EquipmentVM>().Subject;

            returnedModel.HallList.Should().NotBeEmpty();
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
        {
            var vm = new EquipmentVM { Equipment = new Equipment { Id = 99, Name = "Ghost Asset" } };
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Equipment)null);

            var result = _controller.Upsert(vm);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UpsertPost_AddsNewEquipment_WhenAddMode()
        {
            var vm = new EquipmentVM { Equipment = new Equipment { Id = 0, Name = "Laser Projector" } };

            var result = _controller.Upsert(vm);

            _mockUnitOfWork.Verify(u => u.Equipment.Add(It.Is<Equipment>(e => e.Name == "Laser Projector")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Equipment registered successfully");
        }

        [Fact]
        public void UpsertPost_UpdatesEquipment_WhenEditMode()
        {
            var existingEquipment = new Equipment { Id = 1, Name = "Old Name" };
            var vm = new EquipmentVM { Equipment = new Equipment { Id = 1, Name = "Updated Projector" } };

            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingEquipment);

            var result = _controller.Upsert(vm);

            _mockUnitOfWork.Verify(u => u.Equipment.Update(It.Is<Equipment>(e => e.Name == "Updated Projector")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _controller.TempData["success"].Should().Be("Equipment updated successfully");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero(int? id)
        {
            _controller.Delete(id).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Equipment)null);

            _controller.DeletePOST(99).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_SetsIsDeleted_AndDoesNotCallRemove()
        {
            var existingEquipment = new Equipment { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingEquipment);

            var result = _controller.DeletePOST(1);

            existingEquipment.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.Equipment.Update(existingEquipment), Times.Once);
            _mockUnitOfWork.Verify(u => u.Equipment.Remove(It.IsAny<Equipment>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }


        [Fact]
        public void DeleteAjax_ReturnsJsonFalse_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Equipment)null);

            var result = _controller.DeleteAjax(99);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;

            jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "Error: Asset not found." });
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void DeleteAjax_SetsIsDeletedAndReturnsJsonTrue_WhenEntityExists()
        {
            var existingEquipment = new Equipment { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.Equipment.Get(It.IsAny<Expression<Func<Equipment, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingEquipment);

            var result = _controller.DeleteAjax(1);

            existingEquipment.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.Equipment.Update(existingEquipment), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "Delete Successful" });
        }
    }
}