using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class PatchDeviceDtoValidator : AbstractValidator<PatchDeviceDto>
    {
        public PatchDeviceDtoValidator()
        {
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .MaximumLength(100).WithMessage("Device name cannot exceed 100 characters.");
            });

            When(x => x.LocationId.HasValue, () =>
            {
                RuleFor(x => x.LocationId)
                    .GreaterThan(0).WithMessage("Location ID must be a positive number if provided.");
            });
        }
    }
} 