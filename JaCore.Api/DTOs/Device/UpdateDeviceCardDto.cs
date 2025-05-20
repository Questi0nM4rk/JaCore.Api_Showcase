using System;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateDeviceCardDto
    {
        [Required]
        public long DeviceId { get; set; }

        [Required]
        [StringLength(20)]
        public required string SerialNumber { get; set; }

        [Required]
        public DateTime ActivationDate { get; set; }

        [Required]
        public long SupplierId { get; set; }

        [Required]
        public long ServiceId { get; set; }

        [Required]
        public long MetConfirmationId { get; set; }
    }
} 