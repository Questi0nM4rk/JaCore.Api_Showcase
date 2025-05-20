using System;

namespace JaCore.Api.DTOs.Device
{
    public class DeviceDto
    {
        public long ID { get; set; }
        public required string Name { get; set; }
        public long LocationID { get; set; }
        public bool IsDisabled { get; set; }
        public string? DisabledBy { get; set; }
        public DateTime? DisabledAt { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
        // Optionally, add navigation summaries if needed (e.g., LocationName, DeviceCardSummaryDto)
    }
} 