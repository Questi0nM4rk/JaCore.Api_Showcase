using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class CreateDeviceOperationDto
    {
        // DeviceCardId is typically derived from the route parameter, not part of this DTO
        // [Required]
        // public long DeviceCardId { get; set; }

        public int? OrderNo { get; set; }

        [Required]
        public long TemplateUIElemId { get; set; }

        public bool IsRequired { get; set; } = false;

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public required string Label { get; set; }

        public decimal? Value { get; set; }

        [StringLength(10)]
        public string? Unit { get; set; }
    }
} 