using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Entities
{
    public class Actor : BaseEntity
    {
        public Actor()
        {
            Movies = new List<Movie>();
        }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public string Bio { get; set; }

        public string? ProfilePictureURL { get; set; }

        public virtual List<Movie> Movies { get; set; }
    }
}