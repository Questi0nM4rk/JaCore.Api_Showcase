using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class CreateServiceDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(100)]
        public string? Contact { get; set; }
    }
} 