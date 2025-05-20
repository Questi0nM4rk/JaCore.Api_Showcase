using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class DeviceStatusPatchDto
    {
        [Required]
        public bool IsDisabled { get; set; }
    }
} 