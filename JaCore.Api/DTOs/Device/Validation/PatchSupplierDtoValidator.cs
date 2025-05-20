using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class PatchSupplierDtoValidator : AbstractValidator<PatchSupplierDto>
    {
        public PatchSupplierDtoValidator()
        {
            // For PATCH, rules apply only if the property is provided (not null)
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");
            });

            When(x => x.Contact != null, () =>
            {
                RuleFor(x => x.Contact)
                    .MaximumLength(100).WithMessage("Contact cannot exceed 100 characters.");
            });

            When(x => x.ContactPhone != null, () =>
            {
                RuleFor(x => x.ContactPhone)
                    .MaximumLength(50).WithMessage("Contact phone cannot exceed 50 characters.")
                    .Matches(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$|^$").When(x => !string.IsNullOrEmpty(x.ContactPhone)).WithMessage("Invalid phone number format.");
            });

            When(x => x.ContactEmail != null, () =>
            {
                RuleFor(x => x.ContactEmail)
                    .MaximumLength(100).WithMessage("Contact email cannot exceed 100 characters.")
                    .EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail)).WithMessage("Invalid email address format.");
            });

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address)
                    .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.");
            });
        }
    }
} 