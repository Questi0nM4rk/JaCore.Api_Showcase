using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.Entities.Interfaces
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        [Required]
        [StringLength(36)]
        string CreatedBy { get; set; }
        DateTime ModifiedAt { get; set; }
        [Required]
        [StringLength(36)]
        string ModifiedBy { get; set; }
    }
} 