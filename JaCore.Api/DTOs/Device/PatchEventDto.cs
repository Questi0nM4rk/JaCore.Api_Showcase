using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class PatchEventDto
    {
        public DateTime? EventDateTime { get; set; }

        public int? EventType { get; set; }

        [StringLength(100)]
        public string? EventSource { get; set; }

        [StringLength(50)]
        public string? EventUser { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        // Foreign keys - decide if these can be patched
        public long? DeviceCardId { get; set; }
        public long? DeviceOpId { get; set; }
        public long? WorkProdId { get; set; }
    }
} 