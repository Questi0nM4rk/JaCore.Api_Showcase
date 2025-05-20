using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class PatchDeviceCardDto
    {
        // Device_ID is not patchable for an existing card.

        [StringLength(20)]
        public string? SerialNumber { get; set; }

        public DateTime? ActivationDate { get; set; }

        public long? SupplierId { get; set; }
        public long? ServiceId { get; set; }
        public long? MetConfirmationId { get; set; }
    }
} 