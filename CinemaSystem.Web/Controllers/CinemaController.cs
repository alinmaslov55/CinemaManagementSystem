using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Web.Controllers
{
    public class CinemaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CinemaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Cinema> cinemaList = _unitOfWork.Cinema.GetAll();
            return View(cinemaList);
        }

        // GET: Cinema/Upsert
        public IActionResult Upsert(int? id)
        {
            Cinema cinema = new();
            if (id == null || id == 0)
            {
                return View(cinema);
            }
            else
            {
                cinema = _unitOfWork.Cinema.Get(u => u.Id == id);
                return View(cinema);
            }
        }

        // POST: Cinema/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Cinema obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string cinemaPath = Path.Combine(wwwRootPath, @"images\cinema");

                    if (!string.IsNullOrEmpty(obj.Logo))
                    {
                        // Delete the old image if we are updating
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Logo.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(cinemaPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Logo = @"\images\cinema\" + fileName;
                }

                if (obj.Id == 0)
                {
                    _unitOfWork.Cinema.Add(obj);
                }
                else
                {
                    _unitOfWork.Cinema.Update(obj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Cinema created/updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: Cinema/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Cinema? cinemaFromDb = _unitOfWork.Cinema.Get(u => u.Id == id);

            if (cinemaFromDb == null)
            {
                return NotFound();
            }

            return View(cinemaFromDb);
        }

        // POST: Cinema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            Cinema? obj = _unitOfWork.Cinema.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.IsDeleted = true;
            _unitOfWork.Cinema.Update(obj);

            // Physical File Cleanup is not needed when soft delete
            //if (!string.IsNullOrEmpty(obj.Logo))
            //{
            //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.Logo.TrimStart('\\'));
            //    if (System.IO.File.Exists(oldImagePath))
            //    {
            //        System.IO.File.Delete(oldImagePath);
            //    }
            //}

            //_unitOfWork.Cinema.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Cinema deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
