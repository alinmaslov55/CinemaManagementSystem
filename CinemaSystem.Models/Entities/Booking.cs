using CinemaSystem.Models.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text;

namespace CinemaSystem.Models.Entities
{
    public class Booking : BaseEntity
    {
        public Booking()
        {
            Tickets = new List<Ticket>();
        }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int ShowtimeId { get; set; }
        public virtual Showtime Showtime { get; set; }

        [Required]
        public BookingStatus Status { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public double TotalAmount { get; set; }

        [Required]
        public string ConfirmationCode { get; set; }

        [Required]
        public int LoyaltyPointsEarned { get; set; }

        public virtual List<Ticket> Tickets { get; set; }
    }
}
