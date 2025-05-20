using System;

namespace JaCore.Api.DTOs.Device
{
    public class LocationDto
    {
        public long ID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
    }
} 