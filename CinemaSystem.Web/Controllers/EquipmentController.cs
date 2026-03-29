using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EquipmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var objEquipmentList = _unitOfWork.Equipment.GetAll(includeProperties: "CinemaHall").ToList();
            return View(objEquipmentList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            EquipmentVM equipmentVM = new()
            {
                HallList = _unitOfWork.CinemaHall.GetAll(includeProperties: "Cinema").Select(u => new SelectListItem
                {
                    Text = u.Name + " (" + u.Cinema.Name + ")",
                    Value = u.Id.ToString()
                }),
                Equipment = new Equipment()
                {
                    PurchaseDate = DateTime.Now
                }
            };

            if (id == null || id == 0)
            {
                return View(equipmentVM);
            }
            else
            {
                equipmentVM.Equipment = _unitOfWork.Equipment.Get(u => u.Id == id, includeProperties: "CinemaHall");

                if (equipmentVM.Equipment == null)
                {
                    TempData["error"] = "Error: Asset could not be found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(equipmentVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(EquipmentVM equipmentVM)
        {
            if (ModelState.IsValid)
            {
                if (equipmentVM.Equipment.Id == 0)
                {
                    _unitOfWork.Equipment.Add(equipmentVM.Equipment);
                    TempData["success"] = "Equipment registered successfully";
                }
                else
                {
                    _unitOfWork.Equipment.Update(equipmentVM.Equipment);
                    TempData["success"] = "Equipment updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                equipmentVM.HallList = _unitOfWork.CinemaHall.GetAll(includeProperties: "Cinema").Select(u => new SelectListItem
                {
                    Text = u.Name + " (" + u.Cinema.Name + ")",
                    Value = u.Id.ToString()
                });
                return View(equipmentVM);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objToBeDeleted = _unitOfWork.Equipment.Get(u => u.Id == id);
            if (objToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Equipment.Remove(objToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}