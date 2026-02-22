using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Data.Enums
{
    public enum AgeRating
    {
        G = 1,
        PG,
        [Display(Name = "PG-13")]
        PG13,
        R,
        [Display(Name = "NC-17")]
        NC17
    }
}
