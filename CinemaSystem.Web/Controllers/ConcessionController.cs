using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Securitate obligatorie
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
            Concession concession = new Concession();

            if (id == null || id == 0)
            {
                // Create Mode
                return View(concession);
            }
            else
            {
                // Edit Mode
                concession = _unitOfWork.Concession.Get(u => u.Id == id);
                if (concession == null)
                {
                    return NotFound();
                }
                return View(concession);
            }
        }

        // UPSERT: POST
        [HttpPost]
        public IActionResult Upsert(Concession concession, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string concessionPath = Path.Combine(wwwRootPath, @"images\concessions");

                    // Creăm folderul dacă nu există
                    if (!Directory.Exists(concessionPath))
                    {
                        Directory.CreateDirectory(concessionPath);
                    }

                    // Ștergem imaginea veche dacă suntem în mod Edit
                    if (!string.IsNullOrEmpty(concession.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, concession.ImageUrl.TrimStart('\\', '/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Salvăm imaginea nouă
                    using (var fileStream = new FileStream(Path.Combine(concessionPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    concession.ImageUrl = @"/images/concessions/" + fileName;
                }

                if (concession.Id == 0)
                {
                    _unitOfWork.Concession.Add(concession);
                    TempData["success"] = "Concession item created successfully.";
                }
                else
                {
                    _unitOfWork.Concession.Update(concession);
                    TempData["success"] = "Concession item updated successfully.";
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(concession);
        }

        // DELETE: GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var concession = _unitOfWork.Concession.Get(u => u.Id == id);

            if (concession == null)
            {
                return NotFound();
            }

            return View(concession);
        }

        // DELETE: POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var concession = _unitOfWork.Concession.Get(u => u.Id == id);

            if (concession == null)
            {
                return NotFound();
            }

            // Ștergem imaginea din folderul fizic pentru a nu ocupa spațiu degeaba (Resource Management)
            if (!string.IsNullOrEmpty(concession.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, concession.ImageUrl.TrimStart('\\', '/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Concession.Remove(concession);
            _unitOfWork.Save();

            TempData["success"] = "Concession item deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}