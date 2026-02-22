using System.ComponentModel.DataAnnotations;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.Models.ViewModels
{
    public class SeatSelectionVM
    {
        [Required]
        public int ShowtimeId { get; set; }

        [Required]
        public List<Seat> AvailableSeats { get; set; } = new List<Seat>();

        // Metadata for the UI
        public string? MovieTitle { get; set; }
        public string? CinemaHallName { get; set; }
        public DateTime StartTime { get; set; }
    }
}