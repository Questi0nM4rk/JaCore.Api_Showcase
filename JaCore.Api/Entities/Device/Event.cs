using JaCore.Api.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JaCore.Api.Entities.Device
{
    [Table("Event")]
    public class Event : IAuditable, ISoftDeletable
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DeviceCardId { get; set; }
        [ForeignKey("DeviceCardId")]
        public virtual DeviceCard DeviceCard { get; set; } = null!;

        [Required]
        public int EventType { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        // ISoftDeletable
        public bool IsRemoved { get; set; } = false;
        public DateTime? RemovedAt { get; set; }
        [StringLength(36)]
        public string? RemovedBy { get; set; }

        // IAuditable
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        [StringLength(36)]
        public string CreatedBy { get; set; } = null!;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        [Required]
        [StringLength(36)]
        public string ModifiedBy { get; set; } = null!;
    }
} 