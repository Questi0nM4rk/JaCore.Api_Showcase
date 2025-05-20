using JaCore.Api.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JaCore.Api.Entities.Device
{
    [Table("MetConfirmation")]
    public class MetConfirmation : IAuditable, ISoftDeletable
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Lvl1 { get; set; } = null!;

        [StringLength(100)]
        public string? Lvl2 { get; set; }

        [StringLength(100)]
        public string? Lvl3 { get; set; }

        [StringLength(100)]
        public string? Lvl4 { get; set; }

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

        // Navigation property
        public virtual ICollection<DeviceCard> DeviceCards { get; set; } = new List<DeviceCard>();
    }
} 