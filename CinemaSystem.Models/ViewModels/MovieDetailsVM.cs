using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class MovieDetailsVM
    {
        public Movie Movie { get; set; }

        // Dictionary mapping: Cinema -> Date -> List of Showtimes
        public Dictionary<Cinema, Dictionary<DateTime, List<Showtime>>> ShowtimesByCinema { get; set; }

        // Educational Concept: Computed Properties
        // This calculates the average on the fly, avoiding null reference crashes if there are no reviews.
        public double AverageRating => Movie.Reviews != null && Movie.Reviews.Any()
            ? Math.Round(Movie.Reviews.Average(r => r.Rating), 1)
            : 0.0;
    }
}
