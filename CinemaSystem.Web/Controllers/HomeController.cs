using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
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
        private readonly IOllamaService _ollamaService;

        public HomeController(IUnitOfWork unitOfWork, IOllamaService ollamaService)
        {
            _unitOfWork = unitOfWork;
            _ollamaService = ollamaService;
        }

        public IActionResult Index(string? searchString, MovieCategory? category, DateTime? selectedDate)
        {
            var today = DateTime.Now.Date;

            var allMovies = _unitOfWork.Movie.GetAll(includeProperties: "Reviews,Showtimes").ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                allMovies = allMovies.Where(m => m.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (category.HasValue)
            {
                allMovies = allMovies.Where(m => m.MovieCategory == category.Value).ToList();
            }

            var nowPlayingRaw = allMovies.Where(m => m.IsReleased && m.StartDate.Date <= today && m.EndDate.Date >= today);
            var comingSoonRaw = allMovies.Where(m => !m.IsReleased || m.StartDate.Date > today);

            if (selectedDate.HasValue)
            {
                nowPlayingRaw = nowPlayingRaw.Where(m => m.Showtimes.Any(s => s.StartTime.Date == selectedDate.Value.Date));
            }

            var nowPlayingCards = nowPlayingRaw.Select(m => new MovieCardVM
            {
                Movie = m,
                ReviewCount = m.Reviews?.Count() ?? 0,
                AverageRating = m.Reviews != null && m.Reviews.Any() ? Math.Round(m.Reviews.Average(r => r.Rating), 1) : 0
            }).ToList();

            var comingSoonCards = comingSoonRaw.Select(m => new MovieCardVM
            {
                Movie = m,
                ReviewCount = m.Reviews?.Count() ?? 0,
                AverageRating = m.Reviews != null && m.Reviews.Any() ? Math.Round(m.Reviews.Average(r => r.Rating), 1) : 0
            }).ToList();

            var vm = new HomeVM
            {
                // Zone 1: Grab up to 3 highly-rated or newest movies for the massive Hero banner
                HeroMovies = nowPlayingCards.OrderByDescending(m => m.AverageRating).Take(3),

                // Zone 3 & 4
                NowPlaying = nowPlayingCards,
                ComingSoon = comingSoonCards,

                // Zone 2 State
                CurrentSearch = searchString,
                CurrentCategory = category,
                SelectedDate = selectedDate
            };

            return View(vm);
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

        [HttpPost]
        public async Task<IActionResult> AskAI([FromBody] string question)
        {
            if (string.IsNullOrWhiteSpace(question)) return BadRequest("Empty question");

            var today = DateTime.Now;
            var nextWeek = today.AddDays(7);

            var upcomingShowtimes = _unitOfWork.Showtime.GetAll(
                s => s.StartTime >= today && s.StartTime <= nextWeek,
                includeProperties: "Movie,CinemaHall,CinemaHall.Cinema"
            ).ToList();

            var concessions = _unitOfWork.Concession.GetAll(c => c.IsActive).ToList();

            var contextBuilder = new System.Text.StringBuilder();

            contextBuilder.AppendLine("--- CURRENT SHOWTIMES (Next 7 Days) ---");

            var groupedShowtimes = upcomingShowtimes.GroupBy(s => s.Movie.Title);
            foreach (var group in groupedShowtimes)
            {
                var movie = group.First().Movie;
                contextBuilder.AppendLine($"\nMOVIE: {movie.Title} (Genre: {movie.MovieCategory}, Rating: {movie.ImdbRating}/5)");

                foreach (var show in group.OrderBy(s => s.StartTime))
                {
                    contextBuilder.AppendLine($"- {show.StartTime.ToString("dddd, MMM dd at HH:mm")} | Location: {show.CinemaHall.Cinema.Name} (Hall: {show.CinemaHall.Name}) | Price: ${(show.Price ?? 0).ToString("F2")}");
                }
            }

            contextBuilder.AppendLine("\n--- AVAILABLE SNACKS & DRINKS (CONCESSIONS) ---");
            foreach (var item in concessions)
            {
                contextBuilder.AppendLine($"- {item.Name}: ${item.Price.ToString("F2")} ({item.Description})");
            }

            var answer = await _ollamaService.GetMovieRecommendationAsync(question, contextBuilder.ToString());

            return Json(new { response = answer });
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