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

                // Map Entity to ViewModel
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
            if (ModelState.IsValid)
            {
                // Map ViewModel back to Entity
                Cinema cinema = new Cinema
                {
                    Id = vm.Id,
                    Name = vm.Name,
                    Description = vm.Description,
                    Address = vm.Address,
                    City = vm.City,
                    Logo = vm.Logo
                };

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // SECURITY FIX: Whitelist file extensions
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                    string extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["error"] = "Invalid file type. Only JPG, PNG, and WEBP are allowed.";
                        return View(vm);
                    }

                    string fileName = Guid.NewGuid().ToString() + extension;
                    string cinemaPath = Path.Combine(wwwRootPath, @"images\cinema");

                    if (!Directory.Exists(cinemaPath)) Directory.CreateDirectory(cinemaPath);

                    if (!string.IsNullOrEmpty(cinema.Logo))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, cinema.Logo.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(cinemaPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    cinema.Logo = @"\images\cinema\" + fileName;
                }

                if (cinema.Id == 0)
                {
                    _unitOfWork.Cinema.Add(cinema);
                }
                else
                {
                    _unitOfWork.Cinema.Update(cinema);
                }

                _unitOfWork.Save();
                TempData["success"] = "Cinema created/updated successfully";
                return RedirectToAction("Index");
            }

            // If validation fails, return the VM so the user can see error messages
            return View(vm);
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
