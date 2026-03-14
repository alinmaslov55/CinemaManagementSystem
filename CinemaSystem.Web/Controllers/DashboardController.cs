using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var today = DateTime.Now.Date;
            var thirtyDaysAgo = today.AddDays(-30);

            // 1. Fetch base data into memory
            var allBookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Tickets").ToList();
            var allShowtimes = _unitOfWork.Showtime.GetAll(includeProperties: "Movie,CinemaHall").ToList();

            // --- 1. KPIs ---
            decimal totalRev = allBookings.Sum(b => b.TotalAmount);
            decimal todayRev = allBookings.Where(b => b.CreatedDate.Date == today).Sum(b => b.TotalAmount);
            int totalTickets = allBookings.Sum(b => b.Tickets?.Count() ?? 0);

            // Occupancy Rate (Past 30 Days)
            var recentShowtimes = allShowtimes.Where(s => s.StartTime >= thirtyDaysAgo && s.StartTime <= DateTime.Now).ToList();
            int totalCapacity = 0;
            int totalTicketsForRecent = 0;

            foreach (var show in recentShowtimes)
            {
                totalCapacity += 50; // Hardcoded capacity fallback

                // OBJECTIVE FIX: Correlate tickets via the in-memory allBookings list
                totalTicketsForRecent += allBookings
                    .Where(b => b.ShowtimeId == show.Id)
                    .Sum(b => b.Tickets?.Count() ?? 0);
            }

            double occupancy = totalCapacity > 0
                ? Math.Round(((double)totalTicketsForRecent / totalCapacity) * 100, 1)
                : 0;

            // --- 2. Bar Chart: Sales By Movie ---
            var salesByMovie = allShowtimes
                .GroupBy(s => s.Movie.Title)
                .Select(g => new
                {
                    Title = g.Key,
                    // OBJECTIVE FIX: Sum tickets by checking bookings linked to these showtimes
                    Tickets = allBookings
                                .Where(b => g.Select(s => s.Id).Contains(b.ShowtimeId))
                                .Sum(b => b.Tickets?.Count() ?? 0)
                })
                .OrderByDescending(x => x.Tickets)
                .Take(5)
                .ToList();

            // --- 3. Line Chart: 7-Day Revenue Trend ---
            var sevenDaysAgo = today.AddDays(-6);

            var revenueTrend = new Dictionary<string, decimal>();
            for (int i = 0; i < 7; i++)
            {
                revenueTrend.Add(sevenDaysAgo.AddDays(i).ToString("MMM dd"), 0);
            }

            var groupedRevenue = allBookings
                .Where(b => b.CreatedDate.Date >= sevenDaysAgo)
                .GroupBy(b => b.CreatedDate.Date)
                .ToList();

            foreach (var group in groupedRevenue)
            {
                revenueTrend[group.Key.ToString("MMM dd")] = group.Sum(b => b.TotalAmount);
            }

            // --- 4. Actionable Alerts (Empty Showtimes next 24 hrs) ---
            var emptyShowtimes = allShowtimes
                .Where(s => s.StartTime >= DateTime.Now && s.StartTime <= DateTime.Now.AddHours(24))
                // OBJECTIVE FIX: Check if no bookings exist for this showtime
                .Where(s => !allBookings.Any(b => b.ShowtimeId == s.Id))
                .Select(s => new AlertDTO
                {
                    MovieTitle = s.Movie.Title,
                    StartTime = s.StartTime,
                    HallName = s.CinemaHall.Name
                })
                .OrderBy(s => s.StartTime)
                .ToList();

            // --- 5. Live Feed (Recent Bookings) ---
            var recentBookings = allBookings
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .Select(b => new RecentBookingDTO
                {
                    ConfirmationCode = b.ConfirmationCode,
                    CustomerName = b.User?.FullName ?? "Guest",
                    Amount = b.TotalAmount,
                    Date = b.CreatedDate
                })
                .ToList();

            var vm = new DashboardVM
            {
                TotalRevenue = totalRev,
                TodayRevenue = todayRev,
                TotalTicketsSold = totalTickets,
                OccupancyRate = occupancy,

                MovieLabels = salesByMovie.Select(s => s.Title).ToArray(),
                MovieSalesData = salesByMovie.Select(s => s.Tickets).ToArray(),

                DateLabels = revenueTrend.Keys.ToArray(),
                RevenueTrendData = revenueTrend.Values.ToArray(),

                EmptyShowtimes = emptyShowtimes,
                RecentBookings = recentBookings
            };

            return View(vm);
        }
    }
}