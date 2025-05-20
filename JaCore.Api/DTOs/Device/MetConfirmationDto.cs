using System;

namespace JaCore.Api.DTOs.Device
{
    public class MetConfirmationDto
    {
        public long ID { get; set; }
        public required string Name { get; set; }
        public required string Lvl1 { get; set; }
        public string? Lvl2 { get; set; }
        public string? Lvl3 { get; set; }
        public string? Lvl4 { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
    }
} 