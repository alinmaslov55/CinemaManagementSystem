using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        public string? Address { get; set; }

        public int LoyaltyPoints { get; set; } = 0;
    }
}