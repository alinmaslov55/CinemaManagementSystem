using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace CinemaSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EquipmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public EquipmentController(IUnitOfWork unitOfWork, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
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
                return View(equipmentVM); // Create
            }
            else
            {
                equipmentVM.Equipment = _unitOfWork.Equipment.Get(u => u.Id == id, includeProperties: "CinemaHall");

                if (equipmentVM.Equipment == null)
                {
                    TempData["error"] = _localizer["Equipment_Error_NotFound"].Value;
                    return RedirectToAction(nameof(Index));
                }

                return View(equipmentVM); // Edit
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(EquipmentVM equipmentVM)
        {
            if (!ModelState.IsValid)
            {
                equipmentVM.HallList = _unitOfWork.CinemaHall.GetAll(includeProperties: "Cinema").Select(u => new SelectListItem
                {
                    Text = u.Name + " (" + u.Cinema.Name + ")",
                    Value = u.Id.ToString()
                });
                return View(equipmentVM);
            }

            Equipment equipmentToSave;

            if (equipmentVM.Equipment.Id == 0)
            {
                equipmentToSave = new Equipment();
            }
            else
            {
                equipmentToSave = _unitOfWork.Equipment.Get(u => u.Id == equipmentVM.Equipment.Id);
                if (equipmentToSave == null) return NotFound();
            }

            equipmentToSave.Name = equipmentVM.Equipment.Name;
            equipmentToSave.SerialNumber = equipmentVM.Equipment.SerialNumber;
            equipmentToSave.Type = equipmentVM.Equipment.Type;
            equipmentToSave.Status = equipmentVM.Equipment.Status;
            equipmentToSave.PurchaseDate = equipmentVM.Equipment.PurchaseDate;
            equipmentToSave.LastMaintenanceDate = equipmentVM.Equipment.LastMaintenanceDate;
            equipmentToSave.NextMaintenanceDate = equipmentVM.Equipment.NextMaintenanceDate;
            equipmentToSave.MaintenanceNotes = equipmentVM.Equipment.MaintenanceNotes;
            equipmentToSave.CinemaHallId = equipmentVM.Equipment.CinemaHallId;

            if (equipmentVM.Equipment.Id == 0)
            {
                _unitOfWork.Equipment.Add(equipmentToSave);
                TempData["success"] = _localizer["Equipment_CreatedSuccess"].Value;
            }
            else
            {
                _unitOfWork.Equipment.Update(equipmentToSave);
                TempData["success"] = _localizer["Equipment_UpdatedSuccess"].Value;
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var obj = _unitOfWork.Equipment.Get(u => u.Id == id, includeProperties: "CinemaHall");
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Equipment.Get(u => u.Id == id);
            if (obj == null) return NotFound();

            obj.IsDeleted = true;
            _unitOfWork.Equipment.Update(obj);
            _unitOfWork.Save();

            TempData["success"] = _localizer["Equipment_ArchivedSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult DeleteAjax(int? id)
        {
            var objToBeDeleted = _unitOfWork.Equipment.Get(u => u.Id == id);
            if (objToBeDeleted == null)
            {
                return Json(new { success = false, message = _localizer["Equipment_Error_NotFoundAjax"].Value });
            }

            objToBeDeleted.IsDeleted = true;
            _unitOfWork.Equipment.Update(objToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = _localizer["Equipment_DeleteSuccessAjax"].Value });
        }
    }
}