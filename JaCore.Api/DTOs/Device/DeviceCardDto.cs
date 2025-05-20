using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class DeviceCardDto
    {
        public long ID { get; set; }
        public long Device_ID { get; set; }
        public required string SerialNumber { get; set; }
        public DateTime ActivationDate { get; set; }
        public long SupplierID { get; set; }
        public long ServiceID { get; set; }
        public long MetConfirmationID { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }

        // Collections of related items might be paginated or loaded on demand by specific endpoints
        // public ICollection<DeviceOperationDto> DeviceOperations { get; set; } = new List<DeviceOperationDto>();
        // public ICollection<EventDto> Events { get; set; } = new List<EventDto>();
    }
} 