using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.ViewModels;
using CinemaSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var today = DateTime.Now.Date;

            var activeMovies = _unitOfWork.Movie.GetAll(
                m => m.StartDate.Date <= today && m.EndDate.Date >= today
            );

            return View(activeMovies);
        }

        public IActionResult Details(int id)
        {
            var movie = _unitOfWork.Movie.Get(m => m.Id == id, includeProperties: "Actors,Reviews");

            if (movie == null) return NotFound();

            var upcomingShowtimes = _unitOfWork.Showtime.GetAll(
                s => s.MovieId == id && s.StartTime > DateTime.Now,
                includeProperties: "CinemaHall.Cinema"
            );

            var groupedShowtimes = upcomingShowtimes
                .GroupBy(s => s.CinemaHall.Cinema)
                .ToDictionary(
                    cinemaGroup => cinemaGroup.Key,
                    cinemaGroup => cinemaGroup
                        .GroupBy(s => s.StartTime.Date)
                        .ToDictionary(
                            dateGroup => dateGroup.Key,
                            dateGroup => dateGroup.OrderBy(s => s.StartTime).ToList()
                        )
                );

            var vm = new MovieDetailsVM
            {
                Movie = movie,
                ShowtimesByCinema = groupedShowtimes
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
