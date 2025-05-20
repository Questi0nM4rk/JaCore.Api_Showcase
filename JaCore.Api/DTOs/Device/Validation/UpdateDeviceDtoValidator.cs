using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class UpdateDeviceDtoValidator : AbstractValidator<UpdateDeviceDto>
    {
        public UpdateDeviceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Device name is required.")
                .MaximumLength(100).WithMessage("Device name cannot exceed 100 characters.");

            RuleFor(x => x.LocationId)
                .GreaterThan(0).WithMessage("Location ID must be a positive number.");

            RuleFor(x => x.IsDisabled)
                .NotNull().WithMessage("IsDisabled is required.");
        }
    }
} 