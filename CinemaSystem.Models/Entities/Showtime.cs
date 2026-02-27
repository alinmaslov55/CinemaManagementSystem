using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class Showtime : BaseEntity
    {
        [ValidateNever]
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        [ValidateNever]
        public int CinemaHallId { get; set; }
        public virtual CinemaHall CinemaHall { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [ValidateNever]
        public DateTime EndTime { get; set; }

        public decimal? Price { get; set; }

        public string? Language { get; set; }

        public string? Subtitles { get; set; }
    }
}