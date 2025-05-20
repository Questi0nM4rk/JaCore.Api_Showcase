using JaCore.Api.Entities.Interfaces;
using JaCore.Api.Entities.Template; // For TemplateUIElem
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JaCore.Api.Entities.Device
{
    [Table("DeviceOperation")]
    public class DeviceOperation : IAuditable, ISoftDeletable
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int OrderNo { get; set; } // Added Order_No

        [Required]
        [ForeignKey("TemplateUIElemId")]
        public long TemplateUIElemId { get; set; }
        public virtual TemplateUIElem TemplateUIElem { get; set; } = null!;

        public bool IsRequired { get; set; } = false;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Label { get; set; } = null!;

        [Column(TypeName = "decimal(5,5)")] // Ensure correct precision from db schema
        public decimal? Value { get; set; }

        [StringLength(10)]
        public string? Unit { get; set; }

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

        // Navigation property for many-to-many with DeviceCard
        public virtual ICollection<DeviceCard> DeviceCards { get; set; } = new List<DeviceCard>();

        // Navigation property for one-to-many with TempStepOperations (DeviceOperationID side)
        public virtual ICollection<Template.TempStepOperations> TempStepOperations { get; set; } = new List<Template.TempStepOperations>();

        public string OperationStatus { get; set; } = string.Empty;
    }
} 