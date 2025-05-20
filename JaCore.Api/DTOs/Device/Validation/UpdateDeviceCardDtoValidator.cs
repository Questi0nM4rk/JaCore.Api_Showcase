using FluentValidation;
using JaCore.Api.DTOs.Device;
using System;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class UpdateDeviceCardDtoValidator : AbstractValidator<UpdateDeviceCardDto>
    {
        public UpdateDeviceCardDtoValidator()
        {
            RuleFor(x => x.DeviceId)
                .GreaterThan(0).WithMessage("Device ID is required and must be a positive number.");

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required.")
                .MaximumLength(20).WithMessage("Serial number cannot exceed 20 characters.");

            RuleFor(x => x.ActivationDate)
                .NotEmpty().WithMessage("Activation date is required.");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier ID is required and must be a positive number.");

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Service ID is required and must be a positive number.");

            RuleFor(x => x.MetConfirmationId)
                .GreaterThan(0).WithMessage("MetConfirmation ID is required and must be a positive number.");
        }
    }
} 