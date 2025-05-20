using FluentValidation;
using JaCore.Api.DTOs.Device;
using System.Linq;

namespace JaCore.Api.DTOs.Device.Validation
{
    // This validator is for a List<OrderedOperationDto>, often handled directly in the service
    // or by validating each item if a specific validator for OrderedOperationDto exists.
    // If UpdateDeviceOperationsOrderDto was a wrapper DTO containing the list, this would be named after it.
    // Assuming the intent is to validate the list directly or that UpdateDeviceOperationsOrderDto was that wrapper.

    public class OrderedOperationDtoValidator : AbstractValidator<List<OrderedOperationDto>> // Changed from UpdateDeviceOperationsOrderDto to List<OrderedOperationDto>
    {
        public OrderedOperationDtoValidator()
        {
            RuleFor(list => list)
                .NotEmpty().WithMessage("The list of operations cannot be empty.")
                .Must(list => list.Select(dto => dto.OrderNo).Distinct().Count() == list.Count)
                .WithMessage("OrderNo must be unique among all operations in the list.")
                .Must(list => list.All(dto => dto.OrderNo > 0))
                .WithMessage("OrderNo must be a positive integer.");

            // If individual OrderedOperationDto needs validation, you could use SetValidator for each item:
            // RuleForEach(list => list).SetValidator(new IndividualOrderedOperationDtoValidator());
            // Where IndividualOrderedOperationDtoValidator is a validator for OrderedOperationDto
        }
    }

    // Example for individual item validation (if needed)
    /*
    public class IndividualOrderedOperationDtoValidator : AbstractValidator<OrderedOperationDto>
    {
        public IndividualOrderedOperationDtoValidator()
        {
            RuleFor(x => x.OperationId).NotEmpty();
            RuleFor(x => x.OrderNo).GreaterThan(0);
        }
    }
    */
} 