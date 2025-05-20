using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
    {
        public CreateSupplierDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");

            RuleFor(x => x.Contact)
                .MaximumLength(100).WithMessage("Contact cannot exceed 100 characters.");

            RuleFor(x => x.ContactPhone)
                .MaximumLength(50).WithMessage("Contact phone cannot exceed 50 characters.")
                // Basic phone number validation (can be improved with regex for specific formats if needed)
                .Matches(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$|^$").When(x => !string.IsNullOrEmpty(x.ContactPhone)).WithMessage("Invalid phone number format.");

            RuleFor(x => x.ContactEmail)
                .MaximumLength(100).WithMessage("Contact email cannot exceed 100 characters.")
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail)).WithMessage("Invalid email address format.");

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.");
        }
    }
} 