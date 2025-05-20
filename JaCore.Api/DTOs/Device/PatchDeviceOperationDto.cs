using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class PatchDeviceOperationDto
    {
        // DeviceCardId is generally not patched for an operation, ID comes from route
        // public long? DeviceCardId { get; set; }

        public int? Order_No { get; set; } // Renamed from OrderNo

        public long? TemplateUIElemId { get; set; } // Changed from TemplateUIElem_ID to UIElem as per doc

        public bool? IsRequired { get; set; }

        [StringLength(100)]
        public string? Name { get; set; } // Using Name instead of OperationName

        [StringLength(50)]
        public string? Label { get; set; }

        public decimal? Value { get; set; }

        [StringLength(10)]
        public string? Unit { get; set; }
    }
} 