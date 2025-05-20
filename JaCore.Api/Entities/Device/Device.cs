using JaCore.Api.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JaCore.Api.Entities.Device
{
    [Table("Device")]
    public class Device : IAuditable, ISoftDeletable, IDisableable
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        // Foreign key for Location
        [ForeignKey("LocationId")]
        public long? LocationId { get; set; }
        public virtual Location? Location { get; set; }

        // IDisableable
        public bool IsDisabled { get; set; } = false;
        [StringLength(36)]
        public string? DisabledBy { get; set; }
        public DateTime? DisabledAt { get; set; }

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

        // Navigation Properties
        public virtual DeviceCard? DeviceCard { get; set; } // One-to-one with DeviceCard
        // If a Device can be bound to multiple WorkOperations over time (not simultaneously active)
        // then this might be an ICollection, or the relationship might be primarily on WorkOperation.
        // Based on dbcreate.sql `WorkOperation.BoundDevice` this is a one-to-many from Device to WorkOperation.
        public virtual ICollection<Work.WorkOperation> WorkOperations { get; set; } = new List<Work.WorkOperation>();
    }
} 