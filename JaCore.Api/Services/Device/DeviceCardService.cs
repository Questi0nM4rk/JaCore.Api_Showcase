using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using JaCore.Api.Helpers;

namespace JaCore.Api.Services.Device
{
    public class DeviceCardService : IDeviceCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceCardService> _logger;

        public DeviceCardService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeviceCardService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<DeviceCardDto>>> GetDeviceCardsByDeviceIdAsync(long deviceId, QueryParametersDto queryParameters, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device cards for DeviceId: {DeviceId}, Query: {@QueryParameters}", deviceId, queryParameters);
            // First, check if the parent device exists and is active
            var parentDevice = await _unitOfWork.Devices.GetByIdAsync(deviceId);
            if (parentDevice == null)
            {
                return Result.Failure<PagedResult<DeviceCardDto>>(ErrorHelper.NotFound($"Parent device with ID '{deviceId}' not found."));
            }

            var pagedResult = await _unitOfWork.DeviceCards.GetAllByDeviceIdAsync(deviceId, queryParameters, includeProperties);
            var deviceCardDtos = _mapper.Map<System.Collections.Generic.List<DeviceCardDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<DeviceCardDto>(deviceCardDtos, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<DeviceCardDto>> GetDeviceCardByIdAsync(long id, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device card by ID: {DeviceCardId}, Include: {Include}", id, includeProperties);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(id, includeProperties);
            if (deviceCard == null) // Repository GetByIdAsync checks IsRemoved/IsDisabled
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.NotFound($"DeviceCard with ID '{id}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceCardDto>(deviceCard));
        }

        public async Task<Result<DeviceCardDto>> GetDeviceCardByIdAndDeviceIdAsync(long cardId, long deviceId, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device card by CardID: {DeviceCardId} and DeviceID: {DeviceId}, Include: {Include}", cardId, deviceId, includeProperties);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAndDeviceIdAsync(cardId, deviceId, includeProperties);
            if (deviceCard == null) // Repository method checks IsRemoved/IsDisabled
            {
                 return Result.Failure<DeviceCardDto>(ErrorHelper.NotFound($"DeviceCard with ID '{cardId}' associated with Device ID '{deviceId}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceCardDto>(deviceCard));
        }

        public async Task<Result<DeviceCardDto>> GetDeviceCardBySerialNumberAsync(string serialNumber, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device card by SerialNumber: {SerialNumber}, Include: {Include}", serialNumber, includeProperties);
            var deviceCard = await _unitOfWork.DeviceCards.GetDeviceCardBySerialNumberAsync(serialNumber, includeProperties);
            if (deviceCard == null)
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.NotFound($"DeviceCard with SerialNumber '{serialNumber}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceCardDto>(deviceCard));
        }

        public async Task<Result<DeviceCardDto>> CreateDeviceCardAsync(CreateDeviceCardDto createDeviceCardDto)
        {
            _logger.LogInformation("Creating new device card: {@CreateDeviceCardDto}", createDeviceCardDto);

            // Validate parent Device exists and is active
            var device = await _unitOfWork.Devices.GetByIdAsync(createDeviceCardDto.DeviceId);
            if (device == null)
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.NotFound($"Device with ID '{createDeviceCardDto.DeviceId}' not found."));
            }
            if (device.IsDisabled)
            {
                 return Result.Failure<DeviceCardDto>(ErrorHelper.Validation($"Device with ID '{createDeviceCardDto.DeviceId}' is disabled. Cannot add a device card."));
            }

            // Check if a DeviceCard already exists for this DeviceId (unique constraint)
            var existingCardForDevice = await _unitOfWork.DeviceCards.FirstOrDefaultAsync(dc => dc.DeviceId == createDeviceCardDto.DeviceId && !dc.IsRemoved);
            if (existingCardForDevice != null)
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.Conflict($"A DeviceCard already exists for Device ID '{createDeviceCardDto.DeviceId}'."));
            }

            if (!await _unitOfWork.DeviceCards.IsCardSerialUniqueAsync(createDeviceCardDto.SerialNumber))
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.Conflict($"DeviceCard with SerialNumber '{createDeviceCardDto.SerialNumber}' already exists."));
            }

            // Validate FKs: SupplierId, ServiceId, MetConfirmationId
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(createDeviceCardDto.SupplierId);
            if (supplier == null || supplier.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Supplier not found or is removed."));
            
            var serviceEntity = await _unitOfWork.ServiceEntities.GetByIdAsync(createDeviceCardDto.ServiceId);
            if (serviceEntity == null || serviceEntity.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Service not found or is removed."));

            var metConfirmation = await _unitOfWork.MetConfirmations.GetByIdAsync(createDeviceCardDto.MetConfirmationId);
            if (metConfirmation == null || metConfirmation.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("MetConfirmation not found or is removed."));

            var deviceCard = _mapper.Map<DeviceCard>(createDeviceCardDto);
            await _unitOfWork.DeviceCards.AddAsync(deviceCard);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceCardDto>(deviceCard));
        }

        public async Task<Result<DeviceCardDto>> UpdateDeviceCardAsync(long id, UpdateDeviceCardDto updateDeviceCardDto)
        {
            _logger.LogInformation("Updating device card ID: {DeviceCardId} with DTO: {@UpdateDeviceCardDto}", id, updateDeviceCardDto);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(id); // Includes IsRemoved/IsDisabled check
            if (deviceCard == null)
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.NotFound($"DeviceCard with ID '{id}' not found."));
            }

            // DeviceId is immutable for an existing card
            if (deviceCard.DeviceId != updateDeviceCardDto.DeviceId)
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Cannot change the parent Device for an existing DeviceCard."));
            }

            if (deviceCard.SerialNumber != updateDeviceCardDto.SerialNumber && !await _unitOfWork.DeviceCards.IsCardSerialUniqueAsync(updateDeviceCardDto.SerialNumber, id))
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.Conflict($"DeviceCard with SerialNumber '{updateDeviceCardDto.SerialNumber}' already exists."));
            }

            // Validate FKs if changed: SupplierId, ServiceId, MetConfirmationId
            if (deviceCard.SupplierId != updateDeviceCardDto.SupplierId)
            {
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(updateDeviceCardDto.SupplierId);
                if (supplier == null || supplier.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Supplier not found or is removed."));
            }
            if (deviceCard.ServiceId != updateDeviceCardDto.ServiceId)
            {
                var serviceEntity = await _unitOfWork.ServiceEntities.GetByIdAsync(updateDeviceCardDto.ServiceId);
                if (serviceEntity == null || serviceEntity.IsRemoved)
                {
                    return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Service not found or is removed."));
                }
            }
            if (deviceCard.MetConfirmationId != updateDeviceCardDto.MetConfirmationId)
            {
                var metConfirmation = await _unitOfWork.MetConfirmations.GetByIdAsync(updateDeviceCardDto.MetConfirmationId);
                if (metConfirmation == null || metConfirmation.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("MetConfirmation not found or is removed."));
            }

            _mapper.Map(updateDeviceCardDto, deviceCard);
            _unitOfWork.DeviceCards.Update(deviceCard);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceCardDto>(deviceCard));
        }

        public async Task<Result<DeviceCardDto>> PatchDeviceCardAsync(long id, PatchDeviceCardDto patchDeviceCardDto)
        {
            _logger.LogInformation("Patching device card ID: {DeviceCardId} with DTO: {@PatchDeviceCardDto}", id, patchDeviceCardDto);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(id);
            if (deviceCard == null || deviceCard.IsRemoved) 
            {
                return Result.Failure<DeviceCardDto>(ErrorHelper.NotFound("DeviceCard not found or is removed."));
            }

            // DeviceId is immutable, so no check for patchDto.DeviceId here.

            // Apply the patch using AutoMapper
            _mapper.Map(patchDeviceCardDto, deviceCard);

            // Validate FKs if they were part of the patch and changed
            if (patchDeviceCardDto.SupplierId.HasValue)
            {
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(patchDeviceCardDto.SupplierId.Value);
                if (supplier == null || supplier.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Supplier not found or is removed when patching."));
            }
            if (patchDeviceCardDto.ServiceId.HasValue)
            {
                var serviceEntity = await _unitOfWork.ServiceEntities.GetByIdAsync(patchDeviceCardDto.ServiceId.Value);
                if (serviceEntity == null || serviceEntity.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("Service not found or is removed when patching."));
            }
            if (patchDeviceCardDto.MetConfirmationId.HasValue)
            {
                var metConfirmation = await _unitOfWork.MetConfirmations.GetByIdAsync(patchDeviceCardDto.MetConfirmationId.Value);
                if (metConfirmation == null || metConfirmation.IsRemoved) return Result.Failure<DeviceCardDto>(ErrorHelper.Validation("MetConfirmation not found or is removed when patching."));
            }

            _unitOfWork.DeviceCards.Update(deviceCard);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceCardDto>(deviceCard));
        }

        public async Task<Result<bool>> DeleteDeviceCardAsync(long id)
        {
            _logger.LogInformation("Attempting to delete device card ID: {DeviceCardId}", id);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(id);
            if (deviceCard == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"DeviceCard with ID '{id}' not found."));
            }

            // Check for active linked DeviceOperations or Events if necessary, although DB cascade should handle them.
            // For application-level checks if cascade is not relied upon for soft-delete scenarios:
            var operations = await _unitOfWork.DeviceOperations.GetDeviceOperationsByDeviceCardIdAsync(id);
            if (operations.Any(op => !op.IsRemoved))
            {
                return Result.Failure<bool>(ErrorHelper.Validation($"DeviceCard with ID '{id}' has active operations and cannot be deleted."));
            }
            var events = await _unitOfWork.Events.GetEventsByDeviceCardIdAsync(id); // Assuming this repo method exists
            if (events.Any(ev => !ev.IsRemoved))
            {
                return Result.Failure<bool>(ErrorHelper.Validation($"DeviceCard with ID '{id}' has active events and cannot be deleted."));
            }


            _unitOfWork.DeviceCards.Remove(deviceCard); // Handles soft delete
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }
    }
} 