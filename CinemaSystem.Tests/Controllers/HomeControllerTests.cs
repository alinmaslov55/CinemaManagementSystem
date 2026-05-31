using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using CinemaSystem.Web.Controllers;
using CinemaSystem.Web.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CinemaSystem.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOllamaService> _mockOllamaService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly HomeController _controller;
        private readonly DateTime _today;

        public HomeControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOllamaService = new Mock<IOllamaService>();
            _today = DateTime.Now.Date;

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            _mockUnitOfWork.Setup(u => u.Movie.GetAll(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>())).Returns(new List<Movie>());
            _mockUnitOfWork.Setup(u => u.Review.Add(It.IsAny<Review>()));
            _mockUnitOfWork.Setup(u => u.Review.Update(It.IsAny<Review>()));

            _controller = new HomeController(_mockUnitOfWork.Object, _mockOllamaService.Object, _mockUserManager.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test-user-123") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public void Index_FiltersMoviesBySearchString()
        {
            var movies = new List<Movie>
            {
                new Movie { Title = "Batman", IsReleased = true, StartDate = _today, EndDate = _today.AddDays(1) },
                new Movie { Title = "Superman", IsReleased = true, StartDate = _today, EndDate = _today.AddDays(1) }
            };
            _mockUnitOfWork.Setup(u => u.Movie.GetAll(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>())).Returns(movies);

            var result = _controller.Index("bat", null, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<HomeVM>().Subject;
            model.NowPlaying.Should().HaveCount(1);
            model.NowPlaying.First().Movie.Title.Should().Be("Batman");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void AddReview_RejectsInvalidRatings(int invalidRating)
        {
            var result = _controller.AddReview(1, invalidRating, "Test");

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Details");
            _controller.TempData["error"].Should().Be("Invalid rating value.");
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddReview_CreatesNewReview_WhenUserHasNotReviewed()
        {
            _mockUnitOfWork.Setup(u => u.Review.Get(It.IsAny<Expression<Func<Review, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns((Review)null);

            var result = _controller.AddReview(1, 5, "Great!");

            _mockUnitOfWork.Verify(u => u.Review.Add(It.Is<Review>(r => r.Rating == 5 && r.ApplicationUserId == "test-user-123")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            _controller.TempData["success"].Should().Be("Thank you for your review!");
        }

        [Fact]
        public void AddReview_UpdatesExistingReview_WhenUserAlreadyReviewed()
        {
            var existingReview = new Review { MovieId = 1, ApplicationUserId = "test-user-123", Rating = 3, Comment = "Old" };
            _mockUnitOfWork.Setup(u => u.Review.Get(It.IsAny<Expression<Func<Review, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(existingReview);

            var result = _controller.AddReview(1, 5, "New Comment");

            _mockUnitOfWork.Verify(u => u.Review.Update(It.Is<Review>(r => r.Rating == 5 && r.Comment == "New Comment")), Times.Once);
            _mockUnitOfWork.Verify(u => u.Review.Add(It.IsAny<Review>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public async Task AskAI_ReturnsBadRequest_WhenQuestionIsEmpty()
        {
            var result = await _controller.AskAI("");
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AskAI_BuildsContextAndReturnsAnswer_WhenValid()
        {
            var showtimes = new List<Showtime>
            {
                new Showtime
                {
                    Movie = new Movie { Title = "AI Movie", MovieCategory = MovieCategory.SciFi },
                    CinemaHall = new CinemaHall { Name = "Hall 1", Cinema = new Cinema { Name = "Cinema Test" } },
                    StartTime = DateTime.Now,
                    Price = 10
                }
            };
            var concessions = new List<FnBProduct> { new FnBProduct { Name = "Popcorn", Price = 5, IsActive = true } };

            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>())).Returns(showtimes);
            _mockUnitOfWork.Setup(u => u.FnBProduct.GetAll(It.IsAny<Expression<Func<FnBProduct, bool>>>(), It.IsAny<string>())).Returns(concessions);

            _mockOllamaService.Setup(s => s.GetMovieRecommendationAsync(It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync("I recommend AI Movie with Popcorn.");

            var result = await _controller.AskAI("What should I watch?");

            _mockOllamaService.Verify(s => s.GetMovieRecommendationAsync("What should I watch?", It.Is<string>(context =>
                context.Contains("AI Movie") && context.Contains("Popcorn")
            )), Times.Once);

            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;

            var responseProperty = jsonResult.Value.GetType().GetProperty("response");
            var responseValue = responseProperty.GetValue(jsonResult.Value, null);
            responseValue.Should().Be("I recommend AI Movie with Popcorn.");
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsInvalid()
        {
            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns((Movie)null);

            var result = await _controller.Details(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_GroupsShowtimesAndCalculatesRating_WhenValid()
        {
            var cinema = new Cinema { Id = 1, Name = "Main Cinema" };
            var hall = new CinemaHall { Id = 1, Name = "Hall A", Cinema = cinema };

            var movie = new Movie
            {
                Id = 1,
                Title = "Test Movie",
                AgeRating = AgeRating.G,
                Reviews = new List<Review> { new Review { Rating = 4 }, new Review { Rating = 5 } }
            };

            var showtimes = new List<Showtime>
    {
        new Showtime { MovieId = 1, StartTime = DateTime.Now.AddHours(2), CinemaHall = hall },
        new Showtime { MovieId = 1, StartTime = DateTime.Now.AddHours(4), CinemaHall = hall }
    };

            _mockUnitOfWork.Setup(u => u.Movie.Get(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(movie);
            _mockUnitOfWork.Setup(u => u.Showtime.GetAll(It.IsAny<Expression<Func<Showtime, bool>>>(), It.IsAny<string>())).Returns(showtimes);

            var result = await _controller.Details(1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<MovieDetailsVM>().Subject;

            model.AverageRating.Should().Be(4.5);
            model.ShowtimesByCinema.Should().ContainKey(cinema);
            model.ShowtimesByCinema[cinema].First().Value.Should().HaveCount(2);
        }

        [Fact]
        public void Index_SplitsMoviesCorrectly_AndPopulatesHeroSection()
        {
            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Active 1", IsReleased = true, StartDate = _today.AddDays(-5), EndDate = _today.AddDays(5), Showtimes = new List<Showtime>(), Reviews = new List<Review> { new Review { Rating = 5 } } },
                new Movie { Id = 2, Title = "Active 2", IsReleased = true, StartDate = _today.AddDays(-2), EndDate = _today.AddDays(5), Showtimes = new List<Showtime>(), Reviews = new List<Review> { new Review { Rating = 4 } } },
                
                new Movie { Id = 3, Title = "Future 1", IsReleased = false, StartDate = _today.AddDays(10), EndDate = _today.AddDays(20), Showtimes = new List<Showtime>() },
                new Movie { Id = 4, Title = "Future 2", IsReleased = false, StartDate = _today.AddDays(2), EndDate = _today.AddDays(15), Showtimes = new List<Showtime>() }
            };

            _mockUnitOfWork.Setup(u => u.Movie.GetAll(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>())).Returns(movies);

            var result = _controller.Index(null, null, null);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<HomeVM>().Subject;

            model.NowPlaying.Should().HaveCount(2);
            model.ComingSoon.Should().HaveCount(2);

            model.HeroMovies.First().Movie.Title.Should().Be("Active 1");
        }
    }
}