using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateDeviceOperationDtoValidator : AbstractValidator<CreateDeviceOperationDto>
    {
        public CreateDeviceOperationDtoValidator()
        {
            // DeviceCardId is not part of CreateDeviceOperationDto, it comes from the route parameter in the controller.

            RuleFor(x => x.OrderNo)
                .GreaterThanOrEqualTo(0).When(x => x.OrderNo.HasValue).WithMessage("Order number must be a non-negative integer if provided.");

            RuleFor(x => x.TemplateUIElemId) // Changed from TemplateUIElemId
                .NotEmpty().WithMessage("TemplateUIElemId (Template UI Element ID) is required.")
                .GreaterThan(0).WithMessage("TemplateUIElemId (Template UI Element ID) must be a positive number.");

            RuleFor(x => x.IsRequired)
                .NotNull().WithMessage("IsRequired field is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Label is required.")
                .MaximumLength(50).WithMessage("Label cannot exceed 50 characters.");

            RuleFor(x => x.Unit)
                .MaximumLength(10).When(x => x.Value.HasValue && !string.IsNullOrEmpty(x.Unit)).WithMessage("Unit cannot exceed 10 characters if a value is provided.");
        }
    }
} 