using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class HomeVM
    {
        // Zone 1: The Hero
        public IEnumerable<MovieCardVM> HeroMovies { get; set; }

        // Zone 3: Primary Feed
        public IEnumerable<MovieCardVM> NowPlaying { get; set; }

        // Zone 4: Secondary Feed
        public IEnumerable<MovieCardVM> ComingSoon { get; set; }

        // Zone 2: Utility Bar State (so the UI remembers what the user selected)
        public string? CurrentSearch { get; set; }
        public MovieCategory? CurrentCategory { get; set; }
        public DateTime? SelectedDate { get; set; }
    }
    public class MovieCardVM
    {
        public Movie Movie { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
