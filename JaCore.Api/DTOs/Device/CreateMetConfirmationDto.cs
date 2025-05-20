using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class CreateMetConfirmationDto
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
    }
} 