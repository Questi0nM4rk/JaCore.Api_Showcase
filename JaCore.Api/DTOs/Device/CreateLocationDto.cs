using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class CreateLocationDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
} 