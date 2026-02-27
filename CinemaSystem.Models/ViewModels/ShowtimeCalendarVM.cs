using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class ShowtimeCalendarVM
    {
        public CinemaHall Hall { get; set; }
        public IEnumerable<Showtime> WeeklyShowtimes { get; set; }
        public IEnumerable<SelectListItem> MovieList { get; set; }

        // Navigation Properties
        public DateTime CurrentWeekStart { get; set; }
        public DateTime PreviousWeekStart => CurrentWeekStart.AddDays(-7);
        public DateTime NextWeekStart => CurrentWeekStart.AddDays(7);

        // Settings
        public int DayStartHour { get; set; } = 9; // 9 AM
        public int DayEndHour { get; set; } = 24;  // Midnight
    }
}
