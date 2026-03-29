using CinemaSystem.Models.Data.Enums;
using CinemaSystem.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.ViewModels
{
    public class SeatSelectionVM
    {
        public Showtime Showtime { get; set; }

        // Dimensions for the CSS Grid
        public int Rows { get; set; }
        public int Cols { get; set; }

        public List<SeatWithStatusDto> Seats { get; set; } = new List<SeatWithStatusDto>();
    }

    public class SeatWithStatusDto
    {
        public int Id { get; set; }
        public string Row { get; set; }
        public int Column { get; set; }
        public SeatType SeatType { get; set; }
        public bool IsAccessible { get; set; }
        public decimal FinalPrice { get; set; }

        public string Status { get; set; }
    }

    public class HoldSeatsRequestDto
    {
        public int ShowtimeId { get; set; }
        public List<int> SelectedSeatIds { get; set; } = new List<int>();
    }
}