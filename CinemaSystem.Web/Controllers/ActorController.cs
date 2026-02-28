using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Web.Controllers
{
    public class ActorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActorController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var actorList = _unitOfWork.Actor.GetAll();
            return View(actorList);
        }

        public IActionResult Upsert(int? id)
        {
            Actor actor = new();
            if (id == null || id == 0) return View(actor);

            actor = _unitOfWork.Actor.Get(u => u.Id == id);
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Actor obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string actorPath = Path.Combine(wwwRootPath, @"images\actor");

                    // Ensure directory exists
                    if (!Directory.Exists(actorPath)) Directory.CreateDirectory(actorPath);

                    if (!string.IsNullOrEmpty(obj.ProfilePictureURL))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.ProfilePictureURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(actorPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.ProfilePictureURL = @"\images\actor\" + fileName;
                }

                if (obj.Id == 0) _unitOfWork.Actor.Add(obj);
                else _unitOfWork.Actor.Update(obj);

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: Actor/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var actorFromDb = _unitOfWork.Actor.Get(u => u.Id == id);
            if (actorFromDb == null)
            {
                return NotFound();
            }

            return View(actorFromDb);
        }

        // POST: Actor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Actor.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.IsDeleted = true;
            _unitOfWork.Actor.Update(obj);

            // Physical File Cleanup
            //if (!string.IsNullOrEmpty(obj.ProfilePictureURL))
            //{
            //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ProfilePictureURL.TrimStart('\\'));
            //    if (System.IO.File.Exists(oldImagePath))
            //    {
            //        System.IO.File.Delete(oldImagePath);
            //    }
            //}

            //_unitOfWork.Actor.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Actor profile archived successfully";
            return RedirectToAction("Index");
        }
    }
}
