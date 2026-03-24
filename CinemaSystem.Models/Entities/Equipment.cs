using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using CinemaSystem.Models.Data.Enums;

namespace CinemaSystem.Models.Entities
{
    public class Equipment : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        [Required]
        public EquipmentType Type { get; set; }

        [Required]
        public EquipmentStatus Status { get; set; } = EquipmentStatus.Operational;

        [Required]
        public DateTime PurchaseDate { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }

        public string? MaintenanceNotes { get; set; }

        [Required]
        public int CinemaHallId { get; set; }
        [ValidateNever]
        public virtual CinemaHall CinemaHall { get; set; }
    }
}