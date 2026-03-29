using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        public string? Address { get; set; }

        public int LoyaltyPoints { get; set; } = 0;


        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public int? PreferredCinemaId { get; set; }

        [ForeignKey("PreferredCinemaId")]
        public virtual Cinema? PreferredCinema { get; set; }


        [NotMapped]
        public string MembershipTier
        {
            get
            {
                if (LoyaltyPoints >= 1000) return "Gold";
                if (LoyaltyPoints >= 500) return "Silver";
                return "Bronze";
            }
        }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}