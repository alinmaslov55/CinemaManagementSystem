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
            if(vm.CinemaHall.Seats != null && vm.CinemaHall.Seats.Any())
{
                var existingLayout = vm.CinemaHall.Seats.Select(s => new SeatLayoutDto
                {
                    Row = s.Row,
                    Col = s.Column,
                    Type = (int)s.SeatType, // Explicit cast to integer
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
            // 1. Validation and Fallbacks
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
                TempData["error"] = "Seat layout could not be generated. Please try again.";
                vm.CinemaList = _unitOfWork.Cinema.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(vm);
            }

            // 2. Prepare Data
            vm.CinemaHall.TotalSeats = vm.Rows * vm.Cols;
            var incomingSeats = JsonSerializer.Deserialize<List<SeatLayoutDto>>(vm.SeatLayoutData);

            // 3. CREATE Logic
            if (vm.CinemaHall.Id == 0)
            {
                _unitOfWork.CinemaHall.Add(vm.CinemaHall);
                _unitOfWork.Save(); // Save to generate the new Hall ID so seats can map to it

                foreach (var item in incomingSeats)
                {
                    _unitOfWork.Seat.Add(new Seat
                    {
                        Row = item.Row,
                        Column = item.Col,
                        SeatType = (SeatType)item.Type,
                        IsAccessible = item.IsAcc,
                        CinemaHallId = vm.CinemaHall.Id
                    });
                }
            }
            // 4. UPDATE Logic
            else
            {
                var hallFromDb = _unitOfWork.CinemaHall.Get(u => u.Id == vm.CinemaHall.Id, includeProperties: "Seats");

                // Safety Check: Prevent dimensions change if hall has scheduled movies
                bool hasActiveShows = _unitOfWork.Showtime.GetAll(s => s.CinemaHallId == hallFromDb.Id).Any();
                bool dimensionsChanged = hallFromDb.TotalSeats != vm.CinemaHall.TotalSeats;

                if (hasActiveShows && dimensionsChanged)
                {
                    TempData["error"] = "Cannot change layout dimensions because showtimes are scheduled for this hall.";
                    return RedirectToAction(nameof(Upsert), new { id = vm.CinemaHall.Id });
                }

                // Smart Sync: Map incoming layout to existing tracked database seats
                foreach (var inc in incomingSeats)
                {
                    var existingSeat = hallFromDb.Seats.FirstOrDefault(s => s.Row == inc.Row && s.Column == inc.Col);

                    if (existingSeat != null)
                    {
                        // Modify properties
                        existingSeat.SeatType = (SeatType)inc.Type;
                        existingSeat.IsAccessible = inc.IsAcc;
                    }
                    else
                    {
                        // Brand new seat
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

                // Clean up deleted seats if the grid was shrunk
                var incomingCoords = incomingSeats.Select(s => $"{s.Row}-{s.Col}").ToList();
                var seatsToRemove = hallFromDb.Seats.Where(s => !incomingCoords.Contains($"{s.Row}-{s.Column}")).ToList();

                if (seatsToRemove.Any())
                {
                    // Just remove them. Do not mess with hallFromDb.Seats.Remove()
                    _unitOfWork.Seat.RemoveRange(seatsToRemove);
                }

                // Update parent Hall properties
                hallFromDb.Name = vm.CinemaHall.Name;
                hallFromDb.HallType = vm.CinemaHall.HallType;
                hallFromDb.TotalSeats = vm.CinemaHall.TotalSeats;

                // CRITICAL FIX: DO NOT call _unitOfWork.CinemaHall.Update(hallFromDb);
                // Entity Framework is already tracking it and knows it changed.
            }

            // 5. Finalize Transaction
            _unitOfWork.Save();
            TempData["success"] = "Hall configured safely.";
            return RedirectToAction("Index");
        }

        // GET: CinemaHall/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Include Cinema to show the location in the confirmation view
            var hallFromDb = _unitOfWork.CinemaHall.Get(u => u.Id == id, includeProperties: "Cinema");

            if (hallFromDb == null)
            {
                return NotFound();
            }

            return View(hallFromDb);
        }

        // POST: CinemaHall/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.CinemaHall.Get(u => u.Id == id, includeProperties: "Seats");

            if (obj == null)
            {
                return NotFound();
            }

            obj.IsDeleted = true;
            _unitOfWork.CinemaHall.Update(obj);

            // Logic Check: You might want to block deletion if showtimes exist
            // if (_unitOfWork.Showtime.Any(u => u.CinemaHallId == id)) { ... }

            //_unitOfWork.CinemaHall.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Cinema Hall archived successfully.";
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