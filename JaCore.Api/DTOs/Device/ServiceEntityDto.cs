using System;

namespace JaCore.Api.DTOs.Device
{
    public class ServiceEntityDto // Renamed from ServiceDto to align with entity name ServiceEntity
    {
        public long ID { get; set; }
        public required string ServiceName { get; set; } // From Service entity: Name
        public string? ServiceDesc { get; set; } // From Service entity: Description
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
    }
} 