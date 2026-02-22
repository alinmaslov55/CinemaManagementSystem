using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Entities
{
    public class Showtime : BaseEntity
    {
        [Required]
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        [Required]
        public int CinemaHallId { get; set; }
        public virtual CinemaHall CinemaHall { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public double? Price { get; set; }

        public string? Language { get; set; }

        public string? Subtitles { get; set; }
    }
}