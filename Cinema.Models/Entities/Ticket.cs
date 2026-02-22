using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Models.Entities
{
    public class Ticket : BaseEntity
    {
        [Required]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        [Required]
        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; }
    }
}
