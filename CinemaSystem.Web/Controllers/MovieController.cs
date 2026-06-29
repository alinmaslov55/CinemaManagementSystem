using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CinemaSystem.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MovieController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMovieSyncService _movieSyncService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMovieSyncService movieSyncService, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _movieSyncService = movieSyncService;
            _localizer = localizer;
        }

        public IActionResult Index(MovieCategory? category, string? releaseStatus)
        {
            var movieList = _unitOfWork.Movie.GetAll()
                                       .Where(m => !m.IsDeleted)
                                       .AsEnumerable();
            if (category.HasValue)
            {
                movieList = movieList.Where(m => m.MovieCategory == category.Value);
            }

            if (!string.IsNullOrEmpty(releaseStatus) && releaseStatus != "All Movies")
            {
                if (releaseStatus == "Now Showing")
                {
                    movieList = movieList.Where(m => m.IsReleased);
                }
                else if (releaseStatus == "Upcoming")
                {
                    movieList = movieList.Where(m => !m.IsReleased);
                }
            }

            ViewBag.CurrentCategory = category;
            ViewBag.CurrentReleaseStatus = releaseStatus;

            return View(movieList.ToList());
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Movie());
            }

            var movie = _unitOfWork.Movie.Get(u => u.Id == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Movie obj, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            Movie movieToSave;
            if (obj.Id == 0)
            {
                movieToSave = new Movie();
            }
            else
            {
                movieToSave = _unitOfWork.Movie.Get(u => u.Id == obj.Id);
                if (movieToSave == null) return NotFound();
            }

            movieToSave.Title = obj.Title;
            movieToSave.Description = obj.Description;
            movieToSave.Price = obj.Price;
            movieToSave.TrailerUrl = obj.TrailerUrl;
            movieToSave.StartDate = obj.StartDate;
            movieToSave.EndDate = obj.EndDate;
            movieToSave.DurationInMinutes = obj.DurationInMinutes;
            movieToSave.MovieCategory = obj.MovieCategory;
            movieToSave.AgeRating = obj.AgeRating;
            movieToSave.IsReleased = obj.IsReleased;

            if (!string.IsNullOrEmpty(movieToSave.Title) && (string.IsNullOrEmpty(movieToSave.ImdbRating) || string.IsNullOrEmpty(movieToSave.RottenTomatoesScore)))
            {
                var ratings = await _movieSyncService.FetchMovieRatingsAsync(movieToSave.Title);

                if (ratings.imdb != null && string.IsNullOrEmpty(movieToSave.ImdbRating))
                {
                    movieToSave.ImdbRating = ratings.imdb;
                }
                if (ratings.rottenTomatoes != null && string.IsNullOrEmpty(movieToSave.RottenTomatoesScore))
                {
                    movieToSave.RottenTomatoesScore = ratings.rottenTomatoes;
                }
            }

            if (file != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["error"] = _localizer["Movie_InvalidFileType"].Value;
                    return View(obj);
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + extension;
                string moviePath = Path.Combine(wwwRootPath, "images", "movie");

                if (!Directory.Exists(moviePath)) Directory.CreateDirectory(moviePath);

                if (!string.IsNullOrEmpty(movieToSave.ImageUrl))
                {
                    var oldPath = Path.Combine(wwwRootPath, movieToSave.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                using (var fileStream = new FileStream(Path.Combine(moviePath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                movieToSave.ImageUrl = "/images/movie/" + fileName;
            }

            if (obj.Id == 0)
            {
                _unitOfWork.Movie.Add(movieToSave);
                TempData["success"] = _localizer["Movie_CreatedSuccess"].Value;
            }
            else
            {
                _unitOfWork.Movie.Update(movieToSave);
                TempData["success"] = _localizer["Movie_UpdatedSuccess"].Value;
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var movieFromDb = _unitOfWork.Movie.Get(u => u.Id == id);
            if (movieFromDb == null) return NotFound();

            return View(movieFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Movie.Get(u => u.Id == id);
            if (obj == null) return NotFound();

            obj.IsDeleted = true;
            _unitOfWork.Movie.Update(obj);
            _unitOfWork.Save();

            TempData["success"] = _localizer["Movie_ArchivedSuccess"].Value;
            return RedirectToAction("Index");
        }
    }
}