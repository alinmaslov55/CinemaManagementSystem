using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.Web.Controllers
{
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
            // Now we don't need to include "Cinema" since it's no longer on the Movie entity
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
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
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

        // GET: Movie/Delete/5
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

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Movie.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.IsDeleted = true;
            _unitOfWork.Movie.Update(obj);
            // img delete not needed for soft delete
            //if (!string.IsNullOrEmpty(obj.ImageUrl))
            //{
            //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            //    if (System.IO.File.Exists(oldImagePath))
            //    {
            //        System.IO.File.Delete(oldImagePath);
            //    }
            //}

            // _unitOfWork.Movie.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Movie archived successfully";
            return RedirectToAction("Index");
        }
    }
}