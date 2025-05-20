using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class UpdateServiceDtoValidator : AbstractValidator<UpdateServiceDto>
    {
        public UpdateServiceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MaximumLength(100).WithMessage("Service name cannot exceed 100 characters.");

            RuleFor(x => x.Contact)
                .MaximumLength(100).WithMessage("Contact information cannot exceed 100 characters.");
        }
    }
} 