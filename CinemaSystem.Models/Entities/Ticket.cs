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

        // HISTORICAL SNAPSHOTS

        [Required]
        public decimal Price { get; set; } // The exact price paid for this specific seat at checkout

        [Required]
        public string Barcode { get; set; } // A unique Guid string for the QR code scanner at the cinema door
    }
}