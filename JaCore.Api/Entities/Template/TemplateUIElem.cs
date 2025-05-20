using JaCore.Api.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JaCore.Api.Entities.Template
{
    [Table("TemplateUIElem")]
    public class TemplateUIElem // : IAuditable, ISoftDeletable - Not in dbcreate.sql for this table
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        public int ElemType { get; set; }

        // According to dbcreate.sql, TemplateUIElem does not have audit or soft delete fields.
        // If they were intended, the db schema and this entity would need updating.

        // Navigation properties - not directly defined from dbcreate.sql but implied by foreign keys in other tables
        // public virtual ICollection<Device.DeviceOperation> DeviceOperations { get; set; } = new List<Device.DeviceOperation>();
        // public virtual ICollection<TemplateOperation> TemplateOperations { get; set; } = new List<TemplateOperation>();
    }
} 