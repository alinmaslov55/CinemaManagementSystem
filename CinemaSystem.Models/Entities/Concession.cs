using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public enum ConcessionCategory
    {
        Popcorn,
        Beverage,
        Candy,
        HotFood,
        Combo
    }

    public class Concession : BaseEntity
    {
        [Required(ErrorMessage = "The item name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be strictly greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public ConcessionCategory Category { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        // Perfect for "Out of Stock" scenarios without breaking old receipts
        public bool IsActive { get; set; } = true;
    }
}