using CinemaSystem.Models.Data.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ShowtimeId { get; set; }
        public virtual Showtime Showtime { get; set; }

        [Required]
        public BookingStatus Status { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string ConfirmationCode { get; set; } // "BK-8F92A"

        [Required]
        public int LoyaltyPointsEarned { get; set; }

        // STRIPE / PAYMENT GATEWAY TRACKING
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        public virtual List<Ticket> Tickets { get; set; }

        [ValidateNever]
        public virtual ICollection<BookingConcession> BookingConcessions { get; set; } = new List<BookingConcession>();
    }
}