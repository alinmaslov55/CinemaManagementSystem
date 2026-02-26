using CinemaSystem.Models.Data.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CinemaSystem.Models.Entities
{
    public class Movie : BaseEntity
    {
        public Movie()
        {
            Actors = new List<Actor>();
            Showtimes = new List<Showtime>();
        }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        public string? TrailerUrl { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int DurationInMinutes { get; set; }

        [Required]
        public MovieCategory MovieCategory { get; set; }

        [Required]
        public AgeRating AgeRating { get; set; }

        public bool IsReleased { get; set; } = false;


        public virtual List<Actor> Actors { get; set; }
        public virtual List<Showtime> Showtimes { get; set; }
    }
}