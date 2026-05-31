using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class BookingFnB : BaseEntity
    {
        [Required]
        public int BookingId { get; set; }
        [ValidateNever]
        public virtual Booking Booking { get; set; }

        [Required]
        public int FnBProductId { get; set; }
        [ValidateNever]
        public virtual FnBProduct FnBProduct { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal PriceAtPurchase { get; set; }
    }
}