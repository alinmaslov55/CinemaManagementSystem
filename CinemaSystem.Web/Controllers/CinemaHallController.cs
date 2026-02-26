using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

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

            // Eager load Seats to get their current types and positions
            vm.CinemaHall = _unitOfWork.CinemaHall.Get(u => u.Id == id, includeProperties: "Seats");

            // Map existing seats to the DTO and serialize to JSON
            if (vm.CinemaHall.Seats != null && vm.CinemaHall.Seats.Any())
            {
                var existingLayout = vm.CinemaHall.Seats.Select(s => new SeatLayoutDto
                {
                    Row = s.Row,
                    Col = s.Column,
                    Type = s.SeatType,
                    IsAcc = s.IsAccessible
                }).ToList();

                vm.SeatLayoutData = JsonSerializer.Serialize(existingLayout);

                // Ensure Rows and Cols in VM match the actual data
                vm.Rows = vm.CinemaHall.Seats.Max(s => s.Row[0] - 64); // Convert 'A' to 1, etc.
                vm.Cols = vm.CinemaHall.Seats.Max(s => s.Column);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CinemaHallVM vm)
        {
            if (ModelState.IsValid)
            {
                vm.CinemaHall.TotalSeats = vm.Rows * vm.Cols;

                if (vm.CinemaHall.Id == 0)
                {
                    _unitOfWork.CinemaHall.Add(vm.CinemaHall);
                    _unitOfWork.Save();
                }
                else
                {
                    var hallFromDb = _unitOfWork.CinemaHall.Get(u => u.Id == vm.CinemaHall.Id, includeProperties: "Seats");
                    _unitOfWork.Seat.RemoveRange(hallFromDb.Seats); // Reset for new layout

                    hallFromDb.Name = vm.CinemaHall.Name;
                    hallFromDb.CinemaId = vm.CinemaHall.CinemaId;
                    hallFromDb.HallType = vm.CinemaHall.HallType;
                    hallFromDb.TotalSeats = vm.CinemaHall.TotalSeats;
                    _unitOfWork.CinemaHall.Update(hallFromDb);
                }

                // Parse the JSON data from the Seat Editor
                var seatData = JsonSerializer.Deserialize<List<SeatLayoutDto>>(vm.SeatLayoutData);

                foreach (var item in seatData)
                {
                    _unitOfWork.Seat.Add(new Seat
                    {
                        Row = item.Row,
                        Column = item.Col,
                        SeatType = item.Type,
                        IsAccessible = item.IsAcc,
                        CinemaHallId = vm.CinemaHall.Id
                    });
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(vm);
        }
    }

    public class SeatLayoutDto
    {
        public string Row { get; set; }
        public int Col { get; set; }
        public SeatType Type { get; set; }
        public bool IsAcc { get; set; }
    }
}