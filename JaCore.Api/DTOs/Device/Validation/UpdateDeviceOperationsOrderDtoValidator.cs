using FluentValidation;
using JaCore.Api.DTOs.Device;
using System.Linq;

namespace JaCore.Api.DTOs.Device.Validation
{
    public class UpdateDeviceOperationsOrderDtoValidator : AbstractValidator<UpdateDeviceOperationsOrderDto>
    {
        public UpdateDeviceOperationsOrderDtoValidator()
        {
            RuleFor(x => x.Operations)
                .NotEmpty().WithMessage("Operations list cannot be empty.")
                .Must(operations => operations.All(op => op != null)).WithMessage("All operations in the list must not be null.")
                .ForEach(operationRule => {
                    operationRule.ChildRules(op => {
                        op.RuleFor(x => x.OperationId).GreaterThan(0);
                        op.RuleFor(x => x.OrderNo).GreaterThanOrEqualTo(0);
                    });
                });
        }
    }
} 