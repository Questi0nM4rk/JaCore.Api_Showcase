using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class CreateDeviceDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public long LocationId { get; set; }

        public bool IsDisabled { get; set; } = false;
    }
} 