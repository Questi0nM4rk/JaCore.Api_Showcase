using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class CreateEventDto
    {
        [Required]
        public long DeviceCardId { get; set; }

        [Required]
        public DateTime EventDateTime { get; set; }

        [Required]
        public int EventType { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        // Removed other optional IDs for now to simplify, can be added back if needed by EventService.CreateEventAsync
        // public long? Device_ID { get; set; }
        // public long? DeviceOp_ID { get; set; }
        // public long? WorkProd_ID { get; set; }

        // Removed EventSource and EventUser for now, align with Event entity simplified fields
        // [StringLength(100)]
        // public string? EventSource { get; set; }
        // 
        // [StringLength(50)]
        // public string? EventUser { get; set; }
    }
} 