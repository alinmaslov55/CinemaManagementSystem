namespace CinemaSystem.Models.ViewModels
{
    public class DashboardVM
    {
        // 1. Top-Level KPIs
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
        public double OccupancyRate { get; set; }

        // 2. Chart Data (Movies)
        public string[] MovieLabels { get; set; }
        public int[] MovieSalesData { get; set; }

        // 3. Chart Data (7-Day Revenue Trend)
        public string[] DateLabels { get; set; }
        public decimal[] RevenueTrendData { get; set; }

        // 4. Actionable Alerts (Empty Showtimes in next 24 hours)
        public List<AlertDTO> EmptyShowtimes { get; set; }

        // 5. Live Feed
        public List<RecentBookingDTO> RecentBookings { get; set; }
    }

    public class AlertDTO
    {
        public string MovieTitle { get; set; }
        public DateTime StartTime { get; set; }
        public string HallName { get; set; }
    }

    public class RecentBookingDTO
    {
        public string ConfirmationCode { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}