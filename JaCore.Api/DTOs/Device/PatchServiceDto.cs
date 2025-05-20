using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class PatchServiceDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? ContactEmail { get; set; }

        [Phone]
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        public string? ServiceDetails { get; set; } // Assuming text for rich details

        public string? Contact { get; set; }
    }
} 