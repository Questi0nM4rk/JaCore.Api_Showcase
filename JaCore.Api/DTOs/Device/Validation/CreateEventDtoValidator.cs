using FluentValidation;
using JaCore.Api.DTOs.Device;
using System;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.DeviceCardId)
                .NotEmpty().WithMessage("Device Card ID is required.")
                .GreaterThan(0).WithMessage("Device Card ID must be a positive number.");

            RuleFor(x => x.EventDateTime)
                .NotEmpty().WithMessage("Event date and time are required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Event date and time cannot be in the future.");

            RuleFor(x => x.EventType)
                .NotEmpty().WithMessage("Event type is required.");

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        }
    }
} 