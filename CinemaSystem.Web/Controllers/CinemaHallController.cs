using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.Web.Controllers
{
    public class CinemaHallController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CinemaHallController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var halls = _unitOfWork.CinemaHall.GetAll(includeProperties: "Cinema");
            return View(halls);
        }

        public IActionResult Upsert(int? id)
        {
            CinemaHallVM vm = new()
            {
                CinemaHall = new CinemaHall(),
                CinemaList = _unitOfWork.Cinema.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            if (id == null || id == 0) return View(vm);

            vm.CinemaHall = _unitOfWork.CinemaHall.Get(u => u.Id == id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CinemaHallVM vm)
        {
            if (ModelState.IsValid)
            {
                // Calculate total seats based on the customized grid
                vm.CinemaHall.TotalSeats = vm.Rows * vm.Cols;

                if (vm.CinemaHall.Id == 0) _unitOfWork.CinemaHall.Add(vm.CinemaHall);
                else _unitOfWork.CinemaHall.Update(vm.CinemaHall);

                _unitOfWork.Save();
                TempData["success"] = "Hall configured successfully";
                return RedirectToAction("Index");
            }

            vm.CinemaList = _unitOfWork.Cinema.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(vm);
        }
    }
}