using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace CinemaSystem.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CinemaHallController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CinemaHallController(IUnitOfWork unitOfWork, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
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

            var hallFromDb = _unitOfWork.CinemaHall.Get(u => u.Id == id, includeProperties: "Seats");
            if (hallFromDb == null) return NotFound();

            vm.CinemaHall = hallFromDb;

            if (vm.CinemaHall.Seats != null && vm.CinemaHall.Seats.Any())
            {
                var existingLayout = vm.CinemaHall.Seats.Select(s => new SeatLayoutDto
                {
                    Row = s.Row,
                    Col = s.Column,
                    Type = (int)s.SeatType,
                    IsAcc = s.IsAccessible
                }).ToList();

                vm.SeatLayoutData = JsonSerializer.Serialize(existingLayout);
                vm.Rows = vm.CinemaHall.Seats.Max(s => s.Row[0] - 64);
                vm.Cols = vm.CinemaHall.Seats.Max(s => s.Column);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CinemaHallVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CinemaList = _unitOfWork.Cinema.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(vm);
            }

            if (string.IsNullOrEmpty(vm.SeatLayoutData))
            {
                TempData["error"] = _localizer["CinemaHall_Error_NoLayout"].Value;
                vm.CinemaList = _unitOfWork.Cinema.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(vm);
            }

            vm.CinemaHall.TotalSeats = vm.Rows * vm.Cols;
            var incomingSeats = JsonSerializer.Deserialize<List<SeatLayoutDto>>(vm.SeatLayoutData);

            if (incomingSeats == null || !incomingSeats.Any())
            {
                TempData["error"] = _localizer["CinemaHall_Error_NoSeatData"].Value;
                vm.CinemaList = _unitOfWork.Cinema.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                return View(vm);
            }

            if (vm.CinemaHall.Id == 0)
            {
                CinemaHall newHall = new CinemaHall
                {
                    Name = vm.CinemaHall.Name,
                    TotalSeats = vm.CinemaHall.TotalSeats,
                    HallType = vm.CinemaHall.HallType,
                    CinemaId = vm.CinemaHall.CinemaId
                };

                _unitOfWork.CinemaHall.Add(newHall);
                _unitOfWork.Save();

                foreach (var item in incomingSeats)
                {
                    _unitOfWork.Seat.Add(new Seat
                    {
                        Row = item.Row,
                        Column = item.Col,
                        SeatType = (SeatType)item.Type,
                        IsAccessible = item.IsAcc,
                        CinemaHallId = newHall.Id
                    });
                }
                TempData["success"] = _localizer["CinemaHall_CreatedSuccess"].Value;
            }
            else
            {
                var hallFromDb = _unitOfWork.CinemaHall.Get(u => u.Id == vm.CinemaHall.Id, includeProperties: "Seats");
                if (hallFromDb == null) return NotFound();

                bool hasActiveShows = _unitOfWork.Showtime.GetAll(s => s.CinemaHallId == hallFromDb.Id).Any();
                bool dimensionsChanged = hallFromDb.TotalSeats != vm.CinemaHall.TotalSeats;

                if (hasActiveShows && dimensionsChanged)
                {
                    TempData["error"] = _localizer["CinemaHall_Error_HasShowtimes"].Value;
                    return RedirectToAction(nameof(Upsert), new { id = vm.CinemaHall.Id });
                }

                foreach (var inc in incomingSeats)
                {
                    var existingSeat = hallFromDb.Seats.FirstOrDefault(s => s.Row == inc.Row && s.Column == inc.Col);

                    if (existingSeat != null)
                    {
                        existingSeat.SeatType = (SeatType)inc.Type;
                        existingSeat.IsAccessible = inc.IsAcc;
                    }
                    else
                    {
                        _unitOfWork.Seat.Add(new Seat
                        {
                            Row = inc.Row,
                            Column = inc.Col,
                            SeatType = (SeatType)inc.Type,
                            IsAccessible = inc.IsAcc,
                            CinemaHallId = hallFromDb.Id
                        });
                    }
                }

                var incomingCoords = incomingSeats.Select(s => $"{s.Row}-{s.Col}").ToList();
                var seatsToRemove = hallFromDb.Seats.Where(s => !incomingCoords.Contains($"{s.Row}-{s.Column}")).ToList();

                if (seatsToRemove.Any())
                {
                    _unitOfWork.Seat.RemoveRange(seatsToRemove);
                }

                hallFromDb.Name = vm.CinemaHall.Name;
                hallFromDb.HallType = vm.CinemaHall.HallType;
                hallFromDb.TotalSeats = vm.CinemaHall.TotalSeats;
                hallFromDb.CinemaId = vm.CinemaHall.CinemaId;

                _unitOfWork.CinemaHall.Update(hallFromDb);
                TempData["success"] = _localizer["CinemaHall_UpdatedSuccess"].Value;
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var hallFromDb = _unitOfWork.CinemaHall.Get(u => u.Id == id, includeProperties: "Cinema");
            if (hallFromDb == null) return NotFound();

            return View(hallFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.CinemaHall.Get(u => u.Id == id, includeProperties: "Seats");
            if (obj == null) return NotFound();

            obj.IsDeleted = true;
            _unitOfWork.CinemaHall.Update(obj);
            _unitOfWork.Save();

            TempData["success"] = _localizer["CinemaHall_ArchivedSuccess"].Value;
            return RedirectToAction("Index");
        }
    }

    public class SeatLayoutDto
    {
        public string Row { get; set; }
        public int Col { get; set; }
        public int Type { get; set; }
        public bool IsAcc { get; set; }
    }
}