using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MovieController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var movieList = _unitOfWork.Movie.GetAll();
            return View(movieList);
        }

        public IActionResult Upsert(int? id)
        {
            Movie movie = new();

            if (id == null || id == 0)
            {
                return View(movie);
            }

            movie = _unitOfWork.Movie.Get(u => u.Id == id);
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Movie obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // SECURITY FIX: Whitelist file extensions
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                    string extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["error"] = "Invalid file type. Only JPG, PNG, and WEBP are allowed.";
                        return View(obj);
                    }

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + extension;
                    string moviePath = Path.Combine(wwwRootPath, @"images\movie");

                    if (!Directory.Exists(moviePath)) Directory.CreateDirectory(moviePath);

                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldPath = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(moviePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.ImageUrl = @"\images\movie\" + fileName;
                }

                if (obj.Id == 0)
                {
                    _unitOfWork.Movie.Add(obj);
                }
                else
                {
                    _unitOfWork.Movie.Update(obj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Movie saved successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var movieFromDb = _unitOfWork.Movie.Get(u => u.Id == id);
            if (movieFromDb == null)
            {
                return NotFound();
            }

            return View(movieFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Movie.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            // Soft delete implementation
            obj.IsDeleted = true;
            _unitOfWork.Movie.Update(obj);
            _unitOfWork.Save();

            TempData["success"] = "Movie archived successfully";
            return RedirectToAction("Index");
        }
    }
}