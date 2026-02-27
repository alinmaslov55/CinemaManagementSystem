using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using CinemaSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.Web.Controllers
{
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
            // Default to the current week's Monday if no date is provided
            DateTime start = weekStart ?? DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek + (int)DayOfWeek.Monday);
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
                CurrentWeekStart = start
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult UpsertAjax([FromBody] Showtime showtime)
        {
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
            showtime.EndTime = showtime.StartTime.AddMinutes(movie.DurationInMinutes + bufferMinutes);

            var existingShows = _unitOfWork.Showtime.GetAll(s =>
                s.CinemaHallId == showtime.CinemaHallId &&
                s.Id != showtime.Id &&
                s.StartTime.Date == showtime.StartTime.Date);

            bool isOverlap = existingShows.Any(s =>
                showtime.StartTime < s.EndTime && s.StartTime < showtime.EndTime);

            if (isOverlap)
            {
                return Json(new
                {
                    success = false,
                    message = "Time Conflict: There is another movie scheduled or the 20-minute cleaning buffer is being violated."
                });
            }

            if (showtime.Id == 0) _unitOfWork.Showtime.Add(showtime);
            else _unitOfWork.Showtime.Update(showtime);

            _unitOfWork.Save();
            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult DeleteAjax(int id)
        {
            var obj = _unitOfWork.Showtime.Get(u => u.Id == id);
            if (obj == null) return Json(new { success = false });
            _unitOfWork.Showtime.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true });
        }
    }
}
