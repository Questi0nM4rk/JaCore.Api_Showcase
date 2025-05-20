using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class PatchDeviceDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        public long? LocationId { get; set; }

        public bool? IsDisabled { get; set; }
    }
} 