using JaCore.Api.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JaCore.Api.Entities.Device
{
    [Table("DeviceCard")]
    public class DeviceCard : IAuditable, ISoftDeletable, IDisableable
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DeviceId { get; set; } // Foreign key property
        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string SerialNumber { get; set; } = null!;

        [Required]
        public DateTime ActivationDate { get; set; }

        [Required]
        public long SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;

        [Required]
        public long ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual ServiceEntity Service { get; set; } = null!; // Use ServiceEntity

        [Required]
        public long MetConfirmationId { get; set; }
        [ForeignKey("MetConfirmationId")]
        public virtual MetConfirmation MetConfirmation { get; set; } = null!;

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
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<DeviceOperation> DeviceOperations { get; set; } = new List<DeviceOperation>(); // M-M through DeviceCard_DeviceOperation
    }
} 