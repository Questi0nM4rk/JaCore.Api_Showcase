using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class UpdateDeviceOperationsOrderDto
    {
        [Required]
        [MinLength(1)]
        public List<OrderedOperationDto> Operations { get; set; } = new List<OrderedOperationDto>();
    }
} 