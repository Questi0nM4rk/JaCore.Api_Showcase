using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class ServiceDto // Corresponds to ServiceEntity
    {
        public long ID { get; set; }
        public required string Name { get; set; }
        public string? Contact { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
    }
} 