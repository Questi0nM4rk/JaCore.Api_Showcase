using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JaCore.Api.Entities.Interfaces;

namespace JaCore.Api.Entities.Device
{
    [Table("Location")]
    public class Location : IAuditable, ISoftDeletable
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        // ISoftDeletable properties
        public bool IsRemoved { get; set; } = false;
        public DateTime? RemovedAt { get; set; }
        [StringLength(36)]
        public string? RemovedBy { get; set; }

        // IAuditable properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        [StringLength(36)]
        public string CreatedBy { get; set; } = null!;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        [Required]
        [StringLength(36)]
        public string ModifiedBy { get; set; } = null!;

        // Navigation property
        public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
    }
} 