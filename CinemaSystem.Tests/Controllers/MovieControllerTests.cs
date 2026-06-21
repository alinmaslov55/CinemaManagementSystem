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
using System.Threading.Tasks;
using CinemaSystem.Web.Controllers;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Hosting;

namespace CinemaSystem.Tests.Controllers
{
    public class MovieControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnv;
        private readonly Mock<IMovieSyncService> _mockMovieSyncService;
        private readonly MovieController _controller;

        public MovieControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockWebHostEnv = new Mock<IWebHostEnvironment>();
            _mockMovieSyncService = new Mock<IMovieSyncService>();

            _mockUnitOfWork.Setup(u => u.Movie.Add(It.IsAny<Movie>()));
            _mockUnitOfWork.Setup(u => u.Movie.Update(It.IsAny<Movie>()));

            _controller = new MovieController(
                _mockUnitOfWork.Object,
                _mockWebHostEnv.Object,
                _mockMovieSyncService.Object
            );
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfMovies()
        {
            var fakeMovies = new List<Movie> { new Movie { Id = 1, Title = "Inception" } };
            _mockUnitOfWork.Setup(u => u.Movie.GetAll(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>()))
                           .Returns(fakeMovies);

            var result = _controller.Index(null, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Movie>>().Subject;
            model.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void UpsertGet_ReturnsViewResult_WithNewMovie_WhenIdIsNullOrZero(int? id)
        {
            var result = _controller.Upsert(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<Movie>().Subject;
            model.Id.Should().Be(0);
        }

        [Fact]
        public void UpsertGet_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Movie)null);

            var result = _controller.Upsert(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UpsertGet_ReturnsViewResult_WithMovie_WhenEntityExists()
        {
            var existingMovie = new Movie { Id = 1, Title = "Matrix" };
            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingMovie);

            var result = _controller.Upsert(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<Movie>().Subject;
            model.Title.Should().Be("Matrix");
        }

        [Fact]
        public async Task UpsertPost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var movie = new Movie { Id = 0 };
            _controller.ModelState.AddModelError("Title", "Title is required");

            var result = await _controller.Upsert(movie, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(movie);

            _mockMovieSyncService.Verify(m => m.FetchMovieRatingsAsync(It.IsAny<string>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public async Task UpsertPost_ReturnsNotFound_WhenEditModeAndEntityDoesNotExist()
        {
            var movie = new Movie { Id = 99, Title = "Ghost Movie" };
            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Movie)null);

            var result = await _controller.Upsert(movie, null);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpsertPost_ReturnsViewResult_AndSetsError_WhenFileTypeIsInvalid()
        {
            var movie = new Movie { Id = 0, Title = "Test Movie" };
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("script.js");

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await _controller.Upsert(movie, mockFile.Object);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            _controller.TempData["error"].Should().Be("Invalid file type. Only JPG, PNG, and WEBP are allowed.");
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public async Task UpsertPost_SyncsApiDataAndAddsMovie_WhenAddModeAndRatingsAreEmpty()
        {
            var movie = new Movie { Id = 0, Title = "Avatar", ImdbRating = null };

            _mockMovieSyncService.Setup(m => m.FetchMovieRatingsAsync("Avatar"))
                                 .ReturnsAsync(("7.8", "82%"));

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await _controller.Upsert(movie, null);

            _mockUnitOfWork.Verify(u => u.Movie.Add(It.Is<Movie>(m =>
                m.Title == "Avatar" &&
                m.ImdbRating == "7.8" &&
                m.RottenTomatoesScore == "82%")), Times.Once);

            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task UpsertPost_SkipsApiSyncAndUpdatesMovie_WhenEditModeAndRatingsExist()
        {
            var movieToUpdate = new Movie { Id = 1, Title = "Dune" };

            var existingMovie = new Movie
            {
                Id = 1,
                Title = "Dune Old",
                ImdbRating = "8.0",
                RottenTomatoesScore = "85%"
            };

            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingMovie);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await _controller.Upsert(movieToUpdate, null);

            _mockMovieSyncService.Verify(m => m.FetchMovieRatingsAsync(It.IsAny<string>()), Times.Never);

            _mockUnitOfWork.Verify(u => u.Movie.Update(It.Is<Movie>(m => m.Title == "Dune")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void DeleteGet_ReturnsNotFound_WhenIdIsNullOrZero()
        {
            var result = _controller.Delete(null);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_ReturnsNotFound_WhenEntityDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns((Movie)null);

            var result = _controller.DeletePOST(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeletePOST_SetsIsDeletedToTrue_AndDoesNotCallRemove()
        {
            var existingMovie = new Movie { Id = 1, IsDeleted = false };
            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .Returns(existingMovie);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = _controller.DeletePOST(1);

            existingMovie.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(u => u.Movie.Update(existingMovie), Times.Once);
            _mockUnitOfWork.Verify(u => u.Movie.Remove(It.IsAny<Movie>()), Times.Never); // Protectia arhitecturala
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }
    }
}