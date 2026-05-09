using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.Data.Enums;

namespace CinemaSystem.Tests.IntegrationTests
{
    public class MovieRepositoryIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _db;
        private readonly UnitOfWork _unitOfWork;

        public MovieRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new ApplicationDbContext(options);

            _unitOfWork = new UnitOfWork(_db);
        }

        [Fact]
        public void UnitOfWork_CanAddAndRetrieveMovie_FromDatabase()
        {
            var newMovie = new Movie
            {
                Title = "Integration Test Movie",
                Description = "Testing the DB",
                DurationInMinutes = 120,
                MovieCategory = MovieCategory.Action,
                ImdbRating = "9.0",
                Price = 15.0m,
                IsReleased = true,
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(10)
            };

            _unitOfWork.Movie.Add(newMovie);
            _unitOfWork.Save();

            var movieFromDb = _unitOfWork.Movie.Get(m => m.Title == "Integration Test Movie");

            movieFromDb.Should().NotBeNull();
            movieFromDb.Id.Should().BeGreaterThan(0);
            movieFromDb.MovieCategory.Should().Be(MovieCategory.Action);
            movieFromDb.Price.Should().Be(15.0m);
        }

        [Fact]
        public void UnitOfWork_CanSoftDeleteMovie_AndFilterItOut()
        {
            var movie = new Movie
            {
                Title = "To Be Deleted",
                Description = "Initial Description",
                DurationInMinutes = 120,
                MovieCategory = CinemaSystem.Models.Data.Enums.MovieCategory.Action,
                Price = 10,
                EndDate = DateTime.Now
            };

            _unitOfWork.Movie.Add(movie);
            _unitOfWork.Save();

            var savedMovie = _unitOfWork.Movie.Get(m => m.Title == "To Be Deleted");
            savedMovie.Description = "Updated Description";
            _unitOfWork.Movie.Update(savedMovie);
            _unitOfWork.Save();

            var updatedMovie = _unitOfWork.Movie.Get(m => m.Id == savedMovie.Id);
            updatedMovie.Description.Should().Be("Updated Description");
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();
        }
    }
}