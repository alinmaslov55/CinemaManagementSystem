using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace CinemaSystem.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ShowtimeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowtimeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var halls = _unitOfWork.CinemaHall.GetAll(includeProperties: "Cinema,Showtimes");
            return View(halls);
        }

        public IActionResult Upsert(int hallId, DateTime? weekStart)
        {
            DateTime start;
            if (weekStart.HasValue)
            {
                start = weekStart.Value.Date;
            }
            else
            {
                int currentDay = (int)DateTime.Now.DayOfWeek;
                if (currentDay == 0) currentDay = 7;

                start = DateTime.Now.Date.AddDays(-currentDay + 1);
            }

            DateTime end = start.AddDays(7);

            ShowtimeCalendarVM vm = new()
            {
                Hall = _unitOfWork.CinemaHall.Get(u => u.Id == hallId, includeProperties: "Cinema"),
                WeeklyShowtimes = _unitOfWork.Showtime.GetAll(
                    s => s.CinemaHallId == hallId && s.StartTime >= start && s.StartTime < end,
                    includeProperties: "Movie"
                ),
                MovieList = _unitOfWork.Movie.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Title,
                    Value = i.Id.ToString()
                }),
                CurrentWeekStart = start,
                DayStartHour = 8,
                DayEndHour = 24
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult UpsertAjax([FromBody] Showtime showtime)
        {
            if (showtime == null) return Json(new { success = false, message = "Invalid data payload." });

            var movie = _unitOfWork.Movie.Get(u => u.Id == showtime.MovieId);
            if (movie == null) return Json(new { success = false, message = "Invalid Movie selection." });

            if (showtime.StartTime.Date < movie.StartDate.Date || showtime.StartTime.Date > movie.EndDate.Date)
            {
                return Json(new
                {
                    success = false,
                    message = $"Conflict: Movie is only available between {movie.StartDate.ToShortDateString()} and {movie.EndDate.ToShortDateString()}."
                });
            }

            const int bufferMinutes = 20;
            DateTime calculatedEndTime = showtime.StartTime.AddMinutes(movie.DurationInMinutes + bufferMinutes);

            var existingShows = _unitOfWork.Showtime.GetAll(s =>
                s.CinemaHallId == showtime.CinemaHallId &&
                s.Id != showtime.Id &&
                s.StartTime.Date == showtime.StartTime.Date);

            bool isOverlap = existingShows.Any(s =>
                showtime.StartTime < s.EndTime && s.StartTime < calculatedEndTime);

            if (isOverlap)
            {
                return Json(new
                {
                    success = false,
                    message = "Time Conflict: There is another movie scheduled or the 20-minute cleaning buffer is being violated."
                });
            }

            if (showtime.Id == 0)
            {
                Showtime newShowtime = new Showtime
                {
                    CinemaHallId = showtime.CinemaHallId,
                    MovieId = showtime.MovieId,
                    StartTime = showtime.StartTime,
                    EndTime = calculatedEndTime,
                    Language = showtime.Language,
                    Price = showtime.Price
                };
                _unitOfWork.Showtime.Add(newShowtime);
            }
            else
            {
                var showtimeToUpdate = _unitOfWork.Showtime.Get(u => u.Id == showtime.Id);
                if (showtimeToUpdate == null) return Json(new { success = false, message = "Showtime not found." });

                bool hasBookings = _unitOfWork.Booking?.GetAll(b => b.ShowtimeId == showtime.Id).Any() ?? false;
                if (hasBookings && (showtimeToUpdate.StartTime != showtime.StartTime || showtimeToUpdate.MovieId != showtime.MovieId))
                {
                    return Json(new { success = false, message = "Operation Denied: Tickets have already been sold for this session. You cannot alter the time or movie." });
                }

                showtimeToUpdate.MovieId = showtime.MovieId;
                showtimeToUpdate.StartTime = showtime.StartTime;
                showtimeToUpdate.EndTime = calculatedEndTime;
                showtimeToUpdate.Language = showtime.Language;
                showtimeToUpdate.Price = showtime.Price;

                _unitOfWork.Showtime.Update(showtimeToUpdate);
            }

            _unitOfWork.Save();
            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult DeleteAjax(int id)
        {
            var obj = _unitOfWork.Showtime.Get(u => u.Id == id);
            if (obj == null) return Json(new { success = false, message = "Showtime not found." });

            bool hasBookings = _unitOfWork.Booking?.GetAll(b => b.ShowtimeId == id && b.Status == BookingStatus.Confirmed).Any() ?? false;
            if (hasBookings)
            {
                return Json(new { success = false, message = "Cannot delete this showtime as active bookings (tickets) exist. Please cancel the bookings first." });
            }

            obj.IsDeleted = true;
            _unitOfWork.Showtime.Update(obj);

            _unitOfWork.Save();
            return Json(new { success = true });
        }
    }
}