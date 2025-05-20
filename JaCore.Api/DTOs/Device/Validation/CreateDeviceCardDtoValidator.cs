using FluentValidation;
using JaCore.Api.DTOs.Device;
using System;
using System.Threading.Tasks;
using JaCore.Api.Repositories.Abstractions;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class CreateDeviceCardDtoValidator : AbstractValidator<CreateDeviceCardDto>
    {
        public CreateDeviceCardDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .GreaterThan(0).WithMessage("Device ID must be a positive number.")
                .MustAsync(async (deviceId, cancellation) => await unitOfWork.Devices.ExistsAsync(d => d.Id == deviceId && !d.IsRemoved && !d.IsDisabled))
                .WithMessage("Device not found or is inactive.");

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required.")
                .MaximumLength(20).WithMessage("Serial number cannot exceed 20 characters.");

            RuleFor(x => x.ActivationDate)
                .NotEmpty().WithMessage("Activation date is required.")
                .Must(date => date <= DateTime.Now).WithMessage("Activation date cannot be in the future.");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Supplier ID is required.")
                .GreaterThan(0).WithMessage("Supplier ID must be a positive number.")
                .MustAsync(async (id, cancellation) => await unitOfWork.Suppliers.ExistsAsync(s => s.Id == id && !s.IsRemoved))
                .WithMessage("Supplier not found or is invalid.");

            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage("Service ID is required.")
                .GreaterThan(0).WithMessage("Service ID must be a positive number.")
                .MustAsync(async (id, cancellation) => await unitOfWork.ServiceEntities.ExistsAsync(s => s.Id == id && !s.IsRemoved))
                .WithMessage("Service not found or is invalid.");

            RuleFor(x => x.MetConfirmationId)
                .NotEmpty().WithMessage("MetConfirmation ID is required.")
                .GreaterThan(0).WithMessage("MetConfirmation ID must be a positive number.")
                .MustAsync(async (id, cancellation) => await unitOfWork.MetConfirmations.ExistsAsync(mc => mc.Id == id && !mc.IsRemoved))
                .WithMessage("MetConfirmation not found or is invalid.");
        }
    }
} 