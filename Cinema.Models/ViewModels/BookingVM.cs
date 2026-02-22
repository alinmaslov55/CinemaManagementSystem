using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.ViewModels
{
    public class BookingVM
    {
        [Required(ErrorMessage = "You must select at least one seat")]
        public List<int> SelectedSeatIds { get; set; }

        [Required]
        public int ShowtimeId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Redeem Loyalty Points")]
        public int? LoyaltyPointsToRedeem { get; set; }

        public double TotalPrice { get; set; }
    }
}