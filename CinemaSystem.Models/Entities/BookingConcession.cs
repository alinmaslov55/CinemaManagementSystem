using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class BookingConcession : BaseEntity
    {
        [Required]
        public int BookingId { get; set; }
        [ValidateNever]
        public virtual Booking Booking { get; set; }

        [Required]
        public int ConcessionId { get; set; }
        [ValidateNever]
        public virtual Concession Concession { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal PriceAtPurchase { get; set; }
    }
}