using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class LinkLocationToDeviceDto
    {
        [Required]
        public long LocationId { get; set; }
    }
} 