using FluentValidation;
using JaCore.Api.DTOs.Device;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class UpdateDeviceOperationDtoValidator : AbstractValidator<UpdateDeviceOperationDto>
    {
        public UpdateDeviceOperationDtoValidator()
        {
            // RuleFor(x => x.Id)  // Id is no longer in the DTO, it comes from the route
            //     .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Order_No)
                .NotEmpty().WithMessage("Order number is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Order number must be a non-negative integer.");

            RuleFor(x => x.TemplateUIElemId)
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
                .MaximumLength(10).When(x => !string.IsNullOrEmpty(x.Unit)).WithMessage("Unit cannot exceed 10 characters.");
        }
    }
} 