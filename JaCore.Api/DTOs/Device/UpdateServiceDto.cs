using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateServiceDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(100)]
        public string? Contact { get; set; }

        public DateTime ModifiedAt { get; set; }

        [Required]
        [StringLength(100)]
        public required string ModifiedBy { get; set; }
    }
} 