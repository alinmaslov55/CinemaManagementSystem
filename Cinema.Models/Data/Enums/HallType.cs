using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Data.Enums
{
    public enum HallType
    {
        Standard = 1,
        IMAX,
        Dolby,
        [Display(Name = "4DX")]
        FourDX
    }
}
