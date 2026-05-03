using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            CinemaVM vm = new();

            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                var cinemaFromDb = _unitOfWork.Cinema.Get(u => u.Id == id);
                if (cinemaFromDb == null) return NotFound();

                vm.Id = cinemaFromDb.Id;
                vm.Name = cinemaFromDb.Name;
                vm.Description = cinemaFromDb.Description;
                vm.Address = cinemaFromDb.Address;
                vm.City = cinemaFromDb.City;
                vm.Logo = cinemaFromDb.Logo;

                return View(vm);
            }
        }

        // POST: Cinema/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CinemaVM vm, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            Cinema cinemaToSave;

            if (vm.Id == 0)
            {
                cinemaToSave = new Cinema();
            }
            else
            {
                cinemaToSave = _unitOfWork.Cinema.Get(u => u.Id == vm.Id);
                if (cinemaToSave == null) return NotFound();
            }

            cinemaToSave.Name = vm.Name;
            cinemaToSave.Description = vm.Description;
            cinemaToSave.Address = vm.Address;
            cinemaToSave.City = vm.City;

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["error"] = "Invalid file type. Only JPG, PNG, and WEBP are allowed.";
                    return View(vm);
                }

                string fileName = Guid.NewGuid().ToString() + extension;
                string cinemaPath = Path.Combine(wwwRootPath, "images", "cinema"); // Cross-platform safe

                if (!Directory.Exists(cinemaPath)) Directory.CreateDirectory(cinemaPath);

                if (!string.IsNullOrEmpty(cinemaToSave.Logo))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, cinemaToSave.Logo.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(cinemaPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                cinemaToSave.Logo = "/images/cinema/" + fileName;
            }

            if (vm.Id == 0)
            {
                _unitOfWork.Cinema.Add(cinemaToSave);
                TempData["success"] = "Cinema created successfully";
            }
            else
            {
                _unitOfWork.Cinema.Update(cinemaToSave);
                TempData["success"] = "Cinema updated successfully";
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        // GET: Cinema/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            Cinema? cinemaFromDb = _unitOfWork.Cinema.Get(u => u.Id == id);
            if (cinemaFromDb == null) return NotFound();

            return View(cinemaFromDb);
        }

        // POST: Cinema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            Cinema? obj = _unitOfWork.Cinema.Get(u => u.Id == id);
            if (obj == null) return NotFound();

            // Soft Delete respectat
            obj.IsDeleted = true;
            _unitOfWork.Cinema.Update(obj);
            _unitOfWork.Save();

            TempData["success"] = "Cinema archived successfully";
            return RedirectToAction("Index");
        }
    }
}