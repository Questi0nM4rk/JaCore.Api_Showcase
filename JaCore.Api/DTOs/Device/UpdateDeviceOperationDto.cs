using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateDeviceOperationDto
    {
        // Id is typically derived from the route parameter, not part of this DTO for an update
        // [Required]
        // public long Id { get; set; }

        [Required]
        public int Order_No { get; set; }

        [Required]
        public long TemplateUIElemId { get; set; }
        
        [Required]
        public bool IsRequired { get; set; }

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