using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateSupplierDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

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

        public DateTime ModifiedAt { get; set; }

        [Required]
        [StringLength(100)]
        public required string ModifiedBy { get; set; }
    }
} 