using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;
using CinemaSystem.Web.Areas.Admin.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.Tests.Controllers
{
    public class ConcessionControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult_WithListOfConcessions()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockWebHostEnv = new Mock<IWebHostEnvironment>();

            var fakeConcessions = new List<Concession>
            {
                new Concession { Id = 1, Name = "Popcorn", Price = 5.99m, IsActive = true },
                new Concession { Id = 2, Name = "Cola", Price = 3.99m, IsActive = true }
            };

            mockUnitOfWork.Setup(u => u.Concession.GetAll(null, null)).Returns(fakeConcessions);

            var controller = new ConcessionController(mockUnitOfWork.Object, mockWebHostEnv.Object);

            var result = controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Concession>>().Subject;

            model.Should().HaveCount(2);
            model.First().Name.Should().Be("Popcorn");
        }
    }
}