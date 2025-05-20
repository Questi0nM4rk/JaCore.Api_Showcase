using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.Entities.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsRemoved { get; set; }
        DateTime? RemovedAt { get; set; }
        [StringLength(36)]
        string? RemovedBy { get; set; }
    }
} 