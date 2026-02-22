using System.ComponentModel.DataAnnotations;
using Cinema.Models.Data.Enums;

namespace Cinema.Models.Entities
{
    public class Seat : BaseEntity
    {
        [Required]
        public string Row { get; set; }

        [Required]
        public int Column { get; set; }

        [Required]
        public SeatType SeatType { get; set; }

        [Required]
        public bool IsAccessible { get; set; }

        [Required]
        public int CinemaHallId { get; set; }
        public virtual CinemaHall CinemaHall { get; set; }
    }
}