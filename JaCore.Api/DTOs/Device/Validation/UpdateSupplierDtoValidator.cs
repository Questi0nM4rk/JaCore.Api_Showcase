using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class UpdateSupplierDtoValidator : AbstractValidator<UpdateSupplierDto>
    {
        public UpdateSupplierDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");

            RuleFor(x => x.Contact)
                .MaximumLength(100).WithMessage("Contact cannot exceed 100 characters.");
        }
    }
} 