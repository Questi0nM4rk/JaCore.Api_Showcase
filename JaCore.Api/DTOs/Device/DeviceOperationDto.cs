using System;

namespace JaCore.Api.DTOs.Device
{
    public class DeviceOperationDto
    {
        public long Id { get; set; }
        public long DeviceCardId { get; set; }
        public int Order_No { get; set; }
        public string? OperationStatus { get; set; }
        public long TemplateUIElemId { get; set; }
        public bool IsRequired { get; set; }
        public required string Name { get; set; }
        public required string Label { get; set; }
        public decimal? Value { get; set; }
        public string? Unit { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
        // Soft delete fields (IsRemoved, RemovedAt, RemovedBy) are typically not included in main DTOs 
        // unless explicitly needed by the client for display of soft-deleted items.
    }
} 