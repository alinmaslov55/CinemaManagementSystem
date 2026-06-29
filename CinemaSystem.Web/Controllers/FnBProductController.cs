using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CinemaSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FnBProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public FnBProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            List<FnBProduct> objFnBProductList = _unitOfWork.FnBProduct.GetAll().ToList();
            return View(objFnBProductList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0) return View(new FnBProduct());

            var concession = _unitOfWork.FnBProduct.Get(u => u.Id == id);
            if (concession == null) return NotFound();
            return View(concession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(FnBProduct concession, IFormFile? file)
        {
            if (!ModelState.IsValid) return View(concession);

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            FnBProduct concessionToSave;

            if (concession.Id == 0) concessionToSave = new FnBProduct();
            else
            {
                concessionToSave = _unitOfWork.FnBProduct.Get(u => u.Id == concession.Id);
                if (concessionToSave == null) return NotFound();
            }

            concessionToSave.Name = concession.Name;
            concessionToSave.Description = concession.Description;
            concessionToSave.Price = concession.Price;
            concessionToSave.Category = concession.Category;
            concessionToSave.IsActive = concession.IsActive;

            if (file != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["error"] = _localizer["FnB_InvalidFileType"].Value;
                    return View(concession);
                }

                string fileName = Guid.NewGuid().ToString() + extension;
                string concessionPath = Path.Combine(wwwRootPath, "images", "concessions");
                if (!Directory.Exists(concessionPath)) Directory.CreateDirectory(concessionPath);

                if (!string.IsNullOrEmpty(concessionToSave.ImageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, concessionToSave.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(concessionPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                concessionToSave.ImageUrl = "/images/concessions/" + fileName;
            }

            if (concession.Id == 0)
            {
                _unitOfWork.FnBProduct.Add(concessionToSave);
                TempData["success"] = _localizer["FnB_CreatedSuccess"].Value;
            }
            else
            {
                _unitOfWork.FnBProduct.Update(concessionToSave);
                TempData["success"] = _localizer["FnB_UpdatedSuccess"].Value;
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var concession = _unitOfWork.FnBProduct.Get(u => u.Id == id);
            return concession == null ? NotFound() : View(concession);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var concession = _unitOfWork.FnBProduct.Get(u => u.Id == id);
            if (concession == null) return NotFound();

            concession.IsDeleted = true;
            _unitOfWork.FnBProduct.Update(concession);
            _unitOfWork.Save();

            TempData["success"] = _localizer["FnB_ArchivedSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}