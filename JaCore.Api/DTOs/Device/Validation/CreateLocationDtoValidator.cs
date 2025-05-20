using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
    {
        public CreateLocationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Location name is required.")
                .MaximumLength(100).WithMessage("Location name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");
        }
    }
} 