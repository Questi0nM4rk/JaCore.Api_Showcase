using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateDeviceDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public long LocationId { get; set; }

        [Required]
        public bool IsDisabled { get; set; }
    }
} 