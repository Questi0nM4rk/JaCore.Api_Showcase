using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    // For PATCH, all properties are optional. Validation (e.g., for Name uniqueness if provided) is handled in the service.
    public class PatchSupplierDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Contact { get; set; }

        [StringLength(50)]
        [Phone]
        public string? ContactPhone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }
    }
} 