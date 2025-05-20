using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Device
{
    public class PatchMetConfirmationDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? Lvl1 { get; set; }
        [StringLength(100)]
        public string? Lvl2 { get; set; }
        [StringLength(100)]
        public string? Lvl3 { get; set; }
        [StringLength(100)]
        public string? Lvl4 { get; set; }

        // ConfirmedAt and ConfirmedByUserId are typically set by the system
        // during a confirmation action, not directly patched.
        // However, if the use case requires manual override or correction,
        // they can be included here. For now, assuming they are not directly patchable.
    }
} 