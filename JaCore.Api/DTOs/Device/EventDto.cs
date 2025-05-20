using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class EventDto
    {
        public long Id { get; set; }
        public long DeviceCardId { get; set; }
        // public DeviceCardSummaryDto? DeviceCard { get; set; } // Optional: if needed for representation
        public int EventType { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public bool IsRemoved { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string? RemovedBy { get; set; }
    }

    // Events are often immutable, so an UpdateEventDto might not be standard.
    // If updates are allowed, it would be similar to CreateEventDto, potentially without DeviceCard_ID.
    // public class UpdateEventDto { ... }
} 