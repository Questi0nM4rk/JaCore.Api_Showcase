using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateDeviceDtoValidator : AbstractValidator<CreateDeviceDto>
    {
        public CreateDeviceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Device name is required.")
                .MaximumLength(100).WithMessage("Device name cannot exceed 100 characters.");

            RuleFor(x => x.LocationId)
                .NotEmpty().WithMessage("Location ID is required.")
                .GreaterThan(0).WithMessage("Location ID must be a positive number.");

            // Foreign key IDs (Location_ID, MetConf_ID) are validated for existence in the service layer.
            // Basic check for > 0 if they are provided.
            // RuleFor(x => x.MetConf_ID)
            //     .GreaterThan(0).When(x => x.MetConf_ID.HasValue).WithMessage("MetConfirmation ID must be a positive number.");

            // CreateDeviceDto also has IsDisabled, which defaults to false. 
            // If it needs validation (e.g., must be provided), a rule can be added.
            // For now, assuming default is fine and no explicit validation needed here.
        }
    }
} 