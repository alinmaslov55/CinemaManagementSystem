using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class Ticket : BaseEntity
    {
        [Required]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        [Required]
        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Barcode { get; set; }
    }
}