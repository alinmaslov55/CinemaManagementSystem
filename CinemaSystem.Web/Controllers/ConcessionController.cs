using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ConcessionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConcessionController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Concession> objConcessionList = _unitOfWork.Concession.GetAll().ToList();
            return View(objConcessionList);
        }

        // UPSERT: GET
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create Mode
                return View(new Concession());
            }
            else
            {
                // Edit Mode
                var concession = _unitOfWork.Concession.Get(u => u.Id == id);
                if (concession == null) return NotFound();
                return View(concession);
            }
        }

        // UPSERT: POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Securitate adaugata: Prevenire CSRF
        public IActionResult Upsert(Concession concession, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(concession); // Early return pe invaliditate
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            Concession concessionToSave;

            // 1. ENTITY TRACKING PROTECTION
            if (concession.Id == 0)
            {
                concessionToSave = new Concession();
            }
            else
            {
                concessionToSave = _unitOfWork.Concession.Get(u => u.Id == concession.Id);
                if (concessionToSave == null) return NotFound();
            }

            // Mapam STRICT proprietatile permise
            concessionToSave.Name = concession.Name;
            concessionToSave.Description = concession.Description;
            concessionToSave.Price = concession.Price;
            concessionToSave.Category = concession.Category;
            concessionToSave.IsActive = concession.IsActive;

            // 2. FILE MANAGEMENT (Cross-Platform Safe)
            if (file != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["error"] = "Invalid file type. Only JPG, PNG, and WEBP are allowed.";
                    return View(concession);
                }

                string fileName = Guid.NewGuid().ToString() + extension;
                string concessionPath = Path.Combine(wwwRootPath, "images", "concessions");

                if (!Directory.Exists(concessionPath)) Directory.CreateDirectory(concessionPath);

                // Ștergem imaginea veche (Edit mode) folosind Path.DirectorySeparatorChar
                if (!string.IsNullOrEmpty(concessionToSave.ImageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, concessionToSave.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(concessionPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Calea Web este MEREU cu /
                concessionToSave.ImageUrl = "/images/concessions/" + fileName;
            }

            // 3. DATABASE SAVE
            if (concession.Id == 0)
            {
                _unitOfWork.Concession.Add(concessionToSave);
                TempData["success"] = "Concession item created successfully.";
            }
            else
            {
                _unitOfWork.Concession.Update(concessionToSave);
                TempData["success"] = "Concession item updated successfully.";
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // DELETE: GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var concession = _unitOfWork.Concession.Get(u => u.Id == id);
            if (concession == null) return NotFound();

            return View(concession);
        }

        // DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Securitate adaugata: Prevenire CSRF
        public IActionResult DeletePOST(int? id)
        {
            var concession = _unitOfWork.Concession.Get(u => u.Id == id);
            if (concession == null) return NotFound();

            // Soft Delete recomandat pentru istoricul de comenzi (Bookings/Tickets). 
            // Daca ștergem fizic, toate biletele trecute care conțineau acest produs vor crăpa la accesare!
            concession.IsDeleted = true;
            _unitOfWork.Concession.Update(concession);
            _unitOfWork.Save();

            TempData["success"] = "Concession item archived successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}