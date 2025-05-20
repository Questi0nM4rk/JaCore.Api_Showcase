using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.Entities.Interfaces
{
    public interface IDisableable
    {
        bool IsDisabled { get; set; }
        DateTime? DisabledAt { get; set; }
        [StringLength(36)]
        string? DisabledBy { get; set; }
    }
} 