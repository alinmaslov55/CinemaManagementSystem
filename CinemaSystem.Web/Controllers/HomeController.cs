using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace CinemaSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var today = DateTime.Now.Date;

            var activeMovies = _unitOfWork.Movie.GetAll(
                m => m.StartDate.Date <= today && m.EndDate.Date >= today && m.IsReleased
            );

            return View(activeMovies);
        }

        public IActionResult Details(int id)
        {
            var movie = _unitOfWork.Movie.Get(m => m.Id == id, includeProperties: "Reviews,Reviews.ApplicationUser");

            if (movie == null) return NotFound();

            var upcomingShowtimes = _unitOfWork.Showtime.GetAll(
                s => s.MovieId == id && s.StartTime > DateTime.Now,
                includeProperties: "CinemaHall.Cinema"
            );

            var groupedShowtimes = upcomingShowtimes
                .GroupBy(s => s.CinemaHall.Cinema)
                .ToDictionary(
                    cinemaGroup => cinemaGroup.Key,
                    cinemaGroup => cinemaGroup
                        .GroupBy(s => s.StartTime.Date)
                        .ToDictionary(
                            dateGroup => dateGroup.Key,
                            dateGroup => dateGroup.OrderBy(s => s.StartTime).ToList()
                        )
                );

            double averageRating = 0;
            if(movie.Reviews != null && movie.Reviews.Any())
            {
                averageRating = movie.Reviews.Average(r => r.Rating);
            }

            var vm = new MovieDetailsVM
            {
                Movie = movie,
                ShowtimesByCinema = groupedShowtimes,
                AverageRating = Math.Round(averageRating, 1)
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int movieId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["error"] = "Invalid rating value.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingReview = _unitOfWork.Review.Get(r => r.MovieId == movieId && r.ApplicationUserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                _unitOfWork.Review.Update(existingReview);
                TempData["success"] = "Your review has been updated.";
            }
            else
            {
                var newReview = new Review
                {
                    MovieId = movieId,
                    ApplicationUserId = userId,
                    Rating = rating,
                    Comment = comment
                };
                _unitOfWork.Review.Add(newReview);
                TempData["success"] = "Thank you for your review!";
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}