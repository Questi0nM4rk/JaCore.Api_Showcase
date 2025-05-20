using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateEventDto
    {
        [Required]
        public DateTime EventDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public required string EventType { get; set; } // Matches CreateEventDto, might differ from EventDto's int type

        [StringLength(100)]
        public string? EventSource { get; set; }

        [StringLength(50)]
        public string? EventUser { get; set; }

        [StringLength(1000)]
        public string? EventDesc { get; set; }

        // Foreign keys - decide if these can be updated
        // Typically, an event's core associations (like Device_ID, DeviceCard_ID) might be immutable after creation.
        // For now, including them as nullable if updates are allowed.
        public long? Device_ID { get; set; }
        // public long? DeviceCard_ID { get; set; } // Usually an event belongs to one card, and this might not change.
        public long? DeviceOp_ID { get; set; }
        public long? WorkProd_ID { get; set; }
    }
} 