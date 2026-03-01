using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CinemaSystem.Models.Entities
{
    public class SeatHold : BaseEntity
    {
        [Required]
        public int ShowtimeId { get; set; }
        public virtual Showtime Showtime { get; set; }

        [Required]
        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public DateTime HoldExpiration { get; set; }
    }
}
