using FluentValidation;
using JaCore.Api.DTOs.Device;
using System;
using JaCore.Api.Repositories.Abstractions;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class PatchDeviceCardDtoValidator : AbstractValidator<PatchDeviceCardDto>
    {
        public PatchDeviceCardDtoValidator(IUnitOfWork unitOfWork)
        {
            When(x => x.SerialNumber != null, () =>
            {
                RuleFor(x => x.SerialNumber)
                    .MaximumLength(20).WithMessage("Serial number cannot exceed 20 characters.");
            });

            // ActivationDate is optional, no specific validation needed unless you want to check for future dates

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).When(x => x.SupplierId.HasValue)
                .MustAsync(async (id, cancellation) => await unitOfWork.Suppliers.ExistsAsync(s => s.Id == id!.Value && !s.IsRemoved))
                .When(x => x.SupplierId.HasValue)
                .WithMessage("Supplier not found or is invalid.");

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).When(x => x.ServiceId.HasValue)
                .MustAsync(async (id, cancellation) => await unitOfWork.ServiceEntities.ExistsAsync(s => s.Id == id!.Value && !s.IsRemoved))
                .When(x => x.ServiceId.HasValue)
                .WithMessage("Service not found or is invalid.");

            RuleFor(x => x.MetConfirmationId)
                .GreaterThan(0).When(x => x.MetConfirmationId.HasValue)
                .MustAsync(async (id, cancellation) => await unitOfWork.MetConfirmations.ExistsAsync(mc => mc.Id == id!.Value && !mc.IsRemoved))
                .When(x => x.MetConfirmationId.HasValue)
                .WithMessage("MetConfirmation not found or is invalid.");
        }
    }
} 