using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateMetConfirmationDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        public required string Lvl1 { get; set; }

        [StringLength(100)]
        public string? Lvl2 { get; set; }

        [StringLength(100)]
        public string? Lvl3 { get; set; }

        [StringLength(100)]
        public string? Lvl4 { get; set; }

        // public DateTime ModifiedAt { get; set; } // Audit fields should be handled by the system

        // [Required]
        // [StringLength(50)]
        // public required string ModifiedBy { get; set; } // Audit fields should be handled by the system
    }
} 