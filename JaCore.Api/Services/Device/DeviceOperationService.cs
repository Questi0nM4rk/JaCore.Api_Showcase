using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaCore.Api.Helpers;

namespace JaCore.Api.Services.Device
{
    public class DeviceOperationService : IDeviceOperationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceOperationService> _logger;

        public DeviceOperationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeviceOperationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<DeviceOperationDto>>> GetDeviceOperationsByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device operations for DeviceCardId: {DeviceCardId}, Query: {@QueryParameters}", deviceCardId, queryParameters);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(deviceCardId);
            if (deviceCard == null) // Check IsRemoved as well, GetByIdAsync should handle this
            {
                return Result.Failure<PagedResult<DeviceOperationDto>>(ErrorHelper.NotFound($"DeviceCard with ID '{deviceCardId}' not found."));
            }

            var pagedResult = await _unitOfWork.DeviceOperations.GetAllByDeviceCardIdAsync(deviceCardId, queryParameters, includeProperties);
            var operationDtos = _mapper.Map<List<DeviceOperationDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<DeviceOperationDto>(operationDtos, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<DeviceOperationDto>> GetDeviceOperationByIdAsync(long id, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device operation by ID: {OperationId}, Include: {Include}", id, includeProperties);
            var operation = await _unitOfWork.DeviceOperations.GetByIdAsync(id, includeProperties);
            if (operation == null) 
            {
                return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"DeviceOperation with ID '{id}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceOperationDto>(operation));
        }

        public async Task<Result<DeviceOperationDto>> GetDeviceOperationByIdAndDeviceCardIdAsync(long operationId, long deviceCardId, string? includeProperties = null)
        {
             _logger.LogInformation("Fetching operation by OperationID: {OperationId} and DeviceCardID: {DeviceCardId}, Include: {Include}", operationId, deviceCardId, includeProperties);
            var operation = await _unitOfWork.DeviceOperations.GetByIdAndDeviceCardIdAsync(operationId, deviceCardId, includeProperties);
            if (operation == null) 
            {
                 return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"DeviceOperation with ID '{operationId}' associated with DeviceCard ID '{deviceCardId}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceOperationDto>(operation));
        }

        public async Task<Result<DeviceOperationDto>> CreateDeviceOperationAsync(long deviceCardId, CreateDeviceOperationDto createDto)
        {
            _logger.LogInformation("Creating new device operation for DeviceCardId {DeviceCardId}: {@CreateDeviceOperationDto}", deviceCardId, createDto);

            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(deviceCardId);
            if (deviceCard == null || deviceCard.IsRemoved)
            {
                return Result.Failure<DeviceOperationDto>(ErrorHelper.Validation("DeviceCard not found or is removed.", 
                    $"DeviceCard with ID '{deviceCardId}' not found/removed. Cannot create operation."));
            }

            var templateUIElem = await _unitOfWork.TemplateUIElems.GetByIdAsync(createDto.TemplateUIElemId);
            if (templateUIElem == null)
            {
                return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"TemplateUIElem with ID '{createDto.TemplateUIElemId}' not found."));
            }
            
            if (!createDto.OrderNo.HasValue) 
            {
                 var existingOps = await _unitOfWork.DeviceOperations.GetDeviceOperationsByDeviceCardIdAsync(deviceCardId);
                 createDto.OrderNo = existingOps.Any() ? existingOps.Max(op => op.OrderNo) + 1 : 1;
            }

            var operation = _mapper.Map<DeviceOperation>(createDto);
            operation.DeviceCards.Add(deviceCard); 

            await _unitOfWork.DeviceOperations.AddAsync(operation);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceOperationDto>(operation));
        }

        public async Task<Result<DeviceOperationDto>> UpdateDeviceOperationAsync(long deviceCardId, long operationId, UpdateDeviceOperationDto updateDto)
        {
            _logger.LogInformation("Updating device operation ID: {OperationId} for DeviceCardId: {DeviceCardId} with DTO: {@UpdateDeviceOperationDto}", operationId, deviceCardId, updateDto);
            var operation = await _unitOfWork.DeviceOperations.GetByIdAndDeviceCardIdAsync(operationId, deviceCardId);
            if (operation == null)
            {
                return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"DeviceOperation with ID '{operationId}' not found for DeviceCard '{deviceCardId}'."));
            }

            if (operation.TemplateUIElemId != updateDto.TemplateUIElemId)
            {
                var templateUIElem = await _unitOfWork.TemplateUIElems.GetByIdAsync(updateDto.TemplateUIElemId);
                if (templateUIElem == null)
                {
                    return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"TemplateUIElem with ID '{updateDto.TemplateUIElemId}' not found."));
                }
            }

            _mapper.Map(updateDto, operation);
            _unitOfWork.DeviceOperations.Update(operation);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceOperationDto>(operation));
        }

        public async Task<Result<DeviceOperationDto>> PatchDeviceOperationAsync(long deviceCardId, long operationId, PatchDeviceOperationDto patchDto)
        {
             _logger.LogInformation("Patching device operation ID: {OperationId} for DeviceCardId: {DeviceCardId} with DTO: {@PatchDeviceOperationDto}", operationId, deviceCardId, patchDto);
            var operation = await _unitOfWork.DeviceOperations.GetByIdAndDeviceCardIdAsync(operationId, deviceCardId);
            if (operation == null)
            {
                return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"DeviceOperation with ID '{operationId}' not found for DeviceCard '{deviceCardId}'."));
            }

            if (patchDto.TemplateUIElemId.HasValue && operation.TemplateUIElemId != patchDto.TemplateUIElemId.Value)
            {
                var templateUIElem = await _unitOfWork.TemplateUIElems.GetByIdAsync(patchDto.TemplateUIElemId.Value);
                if (templateUIElem == null)
                {
                    return Result.Failure<DeviceOperationDto>(ErrorHelper.NotFound($"TemplateUIElem with ID '{patchDto.TemplateUIElemId.Value}' not found."));
                }
            }

            _mapper.Map(patchDto, operation);
            _unitOfWork.DeviceOperations.Update(operation);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceOperationDto>(operation));
        }

        public async Task<Result<bool>> DeleteDeviceOperationAsync(long operationId, long deviceCardId)
        {
            _logger.LogInformation("Attempting to delete device operation ID: {OperationId} for DeviceCardId: {DeviceCardId}", operationId, deviceCardId);
            var operation = await _unitOfWork.DeviceOperations.GetByIdAndDeviceCardIdAsync(operationId, deviceCardId);
            if (operation == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"DeviceOperation with ID '{operationId}' not found for DeviceCard '{deviceCardId}'."));
            }

            var cardToUnlink = operation.DeviceCards.FirstOrDefault(dc => dc.Id == deviceCardId);
            if (cardToUnlink != null)
            {
                operation.DeviceCards.Remove(cardToUnlink);
            }
            _unitOfWork.DeviceOperations.Remove(operation);
            
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }
        
        public async Task<Result<IEnumerable<DeviceOperationDto>>> UpdateDeviceOperationsOrderAsync(long deviceCardId, List<OrderedOperationDto> orderedOperationsDto)
        {
            _logger.LogInformation("Updating order of operations for DeviceCardId: {DeviceCardId}", deviceCardId);

            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(deviceCardId);
            if (deviceCard == null || deviceCard.IsRemoved)
            {
                _logger.LogWarning("UpdateDeviceOperationsOrderAsync: DeviceCard with ID '{DeviceCardId}' not found or is removed.", deviceCardId);
                return Result.Failure<IEnumerable<DeviceOperationDto>>(ErrorHelper.NotFound("DeviceCard not found or is removed.",
                    $"DeviceCard with ID '{deviceCardId}' not found or is removed. Cannot update operations order."));
            }

            // Get all existing operations for this device card
            // This needs to get ALL operations, not just a page, to correctly identify missing/new ones.
            // Assuming DeviceOperations.GetDeviceOperationsByDeviceCardIdAsync gets all, or we need another method.
            // For now, let's assume GetDeviceOperationsByDeviceCardIdAsync is sufficient for getting current ops.
            // var existingOperations = await _unitOfWork.DeviceOperations.GetDeviceOperationsByDeviceCardIdAsync(deviceCardId);

            // A more direct way if we need to check existence based on IDs from DTO
            var operationIdsFromDto = orderedOperationsDto.Select(dto => dto.OperationId).ToList();
            var existingOperations = await _unitOfWork.DeviceOperations.FindAsync(op => operationIdsFromDto.Contains(op.Id) && op.DeviceCards.Any(dc => dc.Id == deviceCardId) && !op.IsRemoved);

            if (existingOperations.Count() != operationIdsFromDto.Count)
            {
                 _logger.LogWarning("UpdateDeviceOperationsOrderAsync: Some operations from DTO not found for DeviceCardId {DeviceCardId}.", deviceCardId);
                 return Result.Failure<IEnumerable<DeviceOperationDto>>(ErrorHelper.Validation("Some operations not found for this device card."));
            }

            var operationLookup = existingOperations.ToDictionary(op => op.Id);
            var updatedOperations = new List<DeviceOperation>();

            foreach (var dto in orderedOperationsDto)
            {
                if (operationLookup.TryGetValue(dto.OperationId, out var operation))
                {
                    if (operation.OrderNo != dto.OrderNo)
                    {
                        operation.OrderNo = dto.OrderNo;
                        _unitOfWork.DeviceOperations.Update(operation);
                        updatedOperations.Add(operation);
                    }
                }
                else
                {
                    _logger.LogWarning("UpdateDeviceOperationsOrderAsync: Operation with ID '{OperationId}' from DTO not found for DeviceCardId {DeviceCardId}.", dto.OperationId, deviceCardId);
                }
            }

            if (!updatedOperations.Any())
            {
                 _logger.LogInformation("UpdateDeviceOperationsOrderAsync: No operations required an order update for DeviceCardId {DeviceCardId}", deviceCardId);
                 var currentOps = await _unitOfWork.DeviceOperations.GetDeviceOperationsByDeviceCardIdAsync(deviceCardId);
                 return Result.Success(_mapper.Map<IEnumerable<DeviceOperationDto>>(currentOps.OrderBy(o => o.OrderNo)));
            }

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Successfully updated order for {Count} operations for DeviceCardId: {DeviceCardId}", updatedOperations.Count, deviceCardId);
            
            var finalOperations = await _unitOfWork.DeviceOperations.GetDeviceOperationsByDeviceCardIdAsync(deviceCardId);
            return Result.Success(_mapper.Map<IEnumerable<DeviceOperationDto>>(finalOperations.OrderBy(o => o.OrderNo)));
        }
    }
}
