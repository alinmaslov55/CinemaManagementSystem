using System.ComponentModel.DataAnnotations;
using CinemaSystem.Models.Data.Enums;

namespace CinemaSystem.Models.Entities
{
    public class CinemaHall : BaseEntity
    {
        public CinemaHall()
        {
            Seats = new List<Seat>();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public HallType HallType { get; set; }

        [Required]
        public int CinemaId { get; set; }
        public virtual Cinema Cinema { get; set; }

        public virtual List<Seat> Seats { get; set; }
    }
}