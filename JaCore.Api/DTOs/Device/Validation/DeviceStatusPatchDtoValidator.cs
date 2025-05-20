using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class DeviceStatusPatchDtoValidator : AbstractValidator<DeviceStatusPatchDto>
    {
        public DeviceStatusPatchDtoValidator()
        {
            // IsDisabled is a boolean, its presence and value are inherently validated by the model binding.
            // No further FluentValidation rules are strictly necessary here unless there were other properties.
            // RuleFor(x => x.IsDisabled).NotNull(); // This is implicitly handled.
        }
    }
} 