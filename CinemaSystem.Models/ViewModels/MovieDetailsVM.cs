using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class MovieDetailsVM
    {
        public Movie Movie { get; set; }
        public Dictionary<Cinema, Dictionary<DateTime, List<Showtime>>> ShowtimesByCinema { get; set; }
        public double AverageRating { get; set; }
        public bool IsEligibleToWatch { get; set; } = true;
        public string? RestrictionReason { get; set; }
    }
}
