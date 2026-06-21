using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using CinemaSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace CinemaSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOllamaService _ollamaService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public HomeController(
            IUnitOfWork unitOfWork,
            IOllamaService ollamaService,
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _ollamaService = ollamaService;
            _userManager = userManager;
            _localizer = localizer;
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

            var nowPlayingRaw = allMovies.Where(m =>
                m.Showtimes.Any(s => s.StartTime.Date >= today) ||
                (m.IsReleased && m.StartDate.Date <= today)
            ).ToList();

            var comingSoonRaw = allMovies.Where(m =>
                (!m.IsReleased && m.StartDate.Date > today) ||
                (m.IsReleased && !m.Showtimes.Any(s => s.StartTime.Date == today) && m.Showtimes.Any(s => s.StartTime.Date > today))
            ).ToList();

            if (selectedDate.HasValue)
            {
                nowPlayingRaw = nowPlayingRaw.Where(m => m.Showtimes.Any(s => s.StartTime.Date == selectedDate.Value.Date)).ToList();
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
                HeroMovies = nowPlayingCards.OrderByDescending(m => m.AverageRating).ThenByDescending(m => m.Movie.Id).Take(3),

                NowPlaying = nowPlayingCards,
                ComingSoon = comingSoonCards,

                CurrentSearch = searchString,
                CurrentCategory = category,
                SelectedDate = selectedDate
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
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
            if (movie.Reviews != null && movie.Reviews.Any())
            {
                averageRating = movie.Reviews.Average(r => r.Rating);
            }

            var vm = new MovieDetailsVM
            {
                Movie = movie,
                ShowtimesByCinema = groupedShowtimes,
                AverageRating = Math.Round(averageRating, 1),
                IsEligibleToWatch = true
            };

            int requiredAge = GetRequiredAge(movie.AgeRating);
            string displayRating = GetEnumDisplayName(movie.AgeRating);

            if (requiredAge > 0)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        if (!user.DateOfBirth.HasValue)
                        {
                            vm.IsEligibleToWatch = false;
                            vm.RestrictionReason = $"This movie is rated {displayRating}. Please update your Date of Birth in your account profile to view showtimes.";
                        }
                        else
                        {
                            int userAge = DateTime.Today.Year - user.DateOfBirth.Value.Year;
                            if (user.DateOfBirth.Value.Date > DateTime.Today.AddYears(-userAge)) userAge--;

                            if (userAge < requiredAge)
                            {
                                vm.IsEligibleToWatch = false;
                                vm.RestrictionReason = $"Access Denied: You must be at least {requiredAge} years old to watch a {displayRating} rated movie.";
                            }
                        }
                    }
                }
                else
                {
                    vm.IsEligibleToWatch = false;
                    vm.RestrictionReason = $"This movie is rated {displayRating}. Please log in to verify your age and access showtimes.";
                }
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int movieId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["error"] = _localizer["Error_InvalidRating"].Value;
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingReview = _unitOfWork.Review.Get(r => r.MovieId == movieId && r.ApplicationUserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                _unitOfWork.Review.Update(existingReview);
                TempData["success"] = _localizer["Success_ReviewUpdated"].Value;
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
                TempData["success"] = _localizer["Success_ReviewAdded"].Value;
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

            var fnBProducts = _unitOfWork.FnBProduct.GetAll(c => c.IsActive).ToList();

            var contextBuilder = new System.Text.StringBuilder();

            contextBuilder.AppendLine("--- USER PROFILE & RESTRICTIONS ---");
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    contextBuilder.AppendLine($"Name: {user.FullName}");

                    if (user.DateOfBirth.HasValue)
                    {
                        int userAge = DateTime.Today.Year - user.DateOfBirth.Value.Year;
                        if (user.DateOfBirth.Value.Date > DateTime.Today.AddYears(-userAge)) userAge--;

                        contextBuilder.AppendLine($"Age: {userAge}");
                        contextBuilder.AppendLine($"CRITICAL RULE: You MUST NOT recommend any movie where the required age rating is higher than {userAge}. Filter them out of your suggestions completely.");
                    }
                    else
                    {
                        contextBuilder.AppendLine("Age: Unknown (Profile incomplete).");
                        contextBuilder.AppendLine("RULE: If recommending an age-restricted movie, remind them they must update their Date of Birth in their profile to book tickets.");
                    }
                }
            }
            else
            {
                contextBuilder.AppendLine("Status: Anonymous Browsing (Not Logged In).");
                contextBuilder.AppendLine("RULE: If you recommend a restricted movie (like R or PG-13), explicitly mention that they will need to log in to verify their age before booking.");
            }

            contextBuilder.AppendLine("\n--- CURRENT SHOWTIMES (Next 7 Days) ---");
            var groupedShowtimes = upcomingShowtimes.GroupBy(s => s.Movie.Title);
            foreach (var group in groupedShowtimes)
            {
                var movie = group.First().Movie;
                contextBuilder.AppendLine($"\nMOVIE: {movie.Title} (Genre: {movie.MovieCategory}, Rating: {movie.ImdbRating}/5, Age Rating: {GetEnumDisplayName(movie.AgeRating)})");

                foreach (var show in group.OrderBy(s => s.StartTime))
                {
                    contextBuilder.AppendLine($"- {show.StartTime.ToString("dddd, MMM dd at HH:mm")} | Location: {show.CinemaHall.Cinema.Name} (Hall: {show.CinemaHall.Name}) | Price: ${(show.Price ?? 0).ToString("F2")}");
                }
            }

            contextBuilder.AppendLine("\n--- AVAILABLE SNACKS & DRINKS (Food & Beverage Products) ---");
            foreach (var item in fnBProducts)
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

        private int GetRequiredAge(AgeRating rating)
        {
            return rating switch
            {
                AgeRating.G => 0,
                AgeRating.PG => 0,
                AgeRating.PG13 => 13,
                AgeRating.R => 17,
                AgeRating.NC17 => 18,
                _ => 0
            };
        }

        private string GetEnumDisplayName(Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field == null) return enumValue.ToString();

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute == null ? enumValue.ToString() : attribute.Name;
        }
    }
}