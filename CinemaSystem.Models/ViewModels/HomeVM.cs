using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<MovieCardVM> HeroMovies { get; set; }

        public IEnumerable<MovieCardVM> NowPlaying { get; set; }

        public IEnumerable<MovieCardVM> ComingSoon { get; set; }

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
