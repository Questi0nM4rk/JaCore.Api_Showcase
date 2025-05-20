using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateMetConfirmationDtoValidator : AbstractValidator<CreateMetConfirmationDto>
    {
        public CreateMetConfirmationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Confirmation name is required.")
                .MaximumLength(100).WithMessage("Confirmation name cannot exceed 100 characters.");

            RuleFor(x => x.Lvl1)
                .NotEmpty().WithMessage("Level 1 is required.")
                .MaximumLength(50).WithMessage("Level 1 cannot exceed 50 characters.");

            RuleFor(x => x.Lvl2)
                .MaximumLength(50).WithMessage("Level 2 cannot exceed 50 characters.");

            RuleFor(x => x.Lvl3)
                .MaximumLength(50).WithMessage("Level 3 cannot exceed 50 characters.");

            RuleFor(x => x.Lvl4)
                .MaximumLength(50).WithMessage("Level 4 cannot exceed 50 characters.");
        }
    }
} 