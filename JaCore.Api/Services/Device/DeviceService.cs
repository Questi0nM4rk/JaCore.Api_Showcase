using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using JaCore.Api.Helpers;

namespace JaCore.Api.Services.Device
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeviceService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<DeviceDto>>> GetAllDevicesAsync(QueryParametersDto queryParameters, long? locationId = null, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching all devices. LocationId: {LocationId}, Query: {@QueryParameters}", locationId, queryParameters);
            var pagedResult = await _unitOfWork.Devices.GetAllAsync(queryParameters, locationId, includeProperties);
            var deviceDtos = _mapper.Map<System.Collections.Generic.List<DeviceDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<DeviceDto>(deviceDtos, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<DeviceDto>> GetDeviceByIdAsync(long id, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device by ID: {DeviceId}, Include: {Include}", id, includeProperties);
            var device = await _unitOfWork.Devices.GetByIdAsync(id, includeProperties);
            if (device == null) // IsRemoved and IsDisabled already checked by repository's GetByIdAsync override
            {
                return Result.Failure<DeviceDto>(ErrorHelper.NotFound($"Device with ID '{id}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceDto>(device));
        }

        public async Task<Result<DeviceDto>> GetDeviceByNameAsync(string name, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching device by Name: {Name}, Include: {Include}", name, includeProperties);
            var device = await _unitOfWork.Devices.GetDeviceByNameAsync(name, includeProperties);
            if (device == null)
            {
                return Result.Failure<DeviceDto>(ErrorHelper.NotFound($"Device with Name '{name}' not found."));
            }
            return Result.Success(_mapper.Map<DeviceDto>(device));
        }

        public async Task<Result<DeviceDto>> CreateDeviceAsync(CreateDeviceDto createDeviceDto)
        {
            _logger.LogInformation("Creating new device: {@CreateDeviceDto}", createDeviceDto);

            // Name uniqueness check (not enforced by DB, but may be business logic)
            if (!await _unitOfWork.Devices.IsNameUniqueAsync(createDeviceDto.Name))
            {
                return Result.Failure<DeviceDto>(ErrorHelper.Conflict($"Device with Name '{createDeviceDto.Name}' already exists."));
            }
            var locationForCreate = await _unitOfWork.Locations.GetByIdAsync(createDeviceDto.LocationId);
            if (locationForCreate == null || locationForCreate.IsRemoved)
            {
                return Result.Failure<DeviceDto>(ErrorHelper.Validation("Location not found or is removed."));
            }

            var device = _mapper.Map<Entities.Device.Device>(createDeviceDto);
            await _unitOfWork.Devices.AddAsync(device);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceDto>(device));
        }

        public async Task<Result<DeviceDto>> UpdateDeviceAsync(long id, UpdateDeviceDto updateDeviceDto)
        {
            _logger.LogInformation("Updating device ID: {DeviceId} with DTO: {@UpdateDeviceDto}", id, updateDeviceDto);
            var device = await _unitOfWork.Devices.GetByIdAsync(id);
            if (device == null || device.IsRemoved) return Result.Failure<DeviceDto>(ErrorHelper.NotFound("Device not found."));

            if (device.LocationId != updateDeviceDto.LocationId)
            {
                var locationForUpdate = await _unitOfWork.Locations.GetByIdAsync(updateDeviceDto.LocationId);
                if (locationForUpdate == null || locationForUpdate.IsRemoved)
                {
                    return Result.Failure<DeviceDto>(ErrorHelper.Validation("Location not found or is removed."));
                }
            }

            // Name uniqueness check
            if (device.Name != updateDeviceDto.Name && !await _unitOfWork.Devices.IsNameUniqueAsync(updateDeviceDto.Name, id.ToString()))
            {
                return Result.Failure<DeviceDto>(ErrorHelper.Conflict($"Device with Name '{updateDeviceDto.Name}' already exists."));
            }

            _mapper.Map(updateDeviceDto, device);
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceDto>(device));
        }

        public async Task<Result<DeviceDto>> PatchDeviceAsync(long id, PatchDeviceDto patchDeviceDto)
        {
            _logger.LogInformation("Patching device ID: {DeviceId} with DTO: {@PatchDeviceDto}", id, patchDeviceDto);
            var device = await _unitOfWork.Devices.GetByIdAsync(id);
            if (device == null || device.IsRemoved)
            {
                return Result.Failure<DeviceDto>(ErrorHelper.NotFound($"Device with ID '{id}' not found."));
            }

            if (patchDeviceDto.Name != null && device.Name != patchDeviceDto.Name && !await _unitOfWork.Devices.IsNameUniqueAsync(patchDeviceDto.Name, id.ToString()))
            {
                return Result.Failure<DeviceDto>(ErrorHelper.Conflict($"Device with Name '{patchDeviceDto.Name}' already exists."));
            }
            if (patchDeviceDto.LocationId.HasValue)
            {
                var locationForPatch = await _unitOfWork.Locations.GetByIdAsync(patchDeviceDto.LocationId.Value);
                if (locationForPatch == null || locationForPatch.IsRemoved)
                {
                    return Result.Failure<DeviceDto>(ErrorHelper.Validation("Location not found or is removed for PATCH."));
                }
            }
            _mapper.Map(patchDeviceDto, device); // AutoMapper handles applying only non-null values for patch
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceDto>(device));
        }

        public async Task<Result<bool>> DeleteDeviceAsync(long id)
        {
            _logger.LogInformation("Attempting to delete device ID: {DeviceId}", id);
            var device = await _unitOfWork.Devices.GetByIdAsync(id); // Already checks IsRemoved/IsDisabled
            if (device == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"Device with ID '{id}' not found."));
            }

            // Check for active device cards
            var activeCards = await _unitOfWork.DeviceCards.GetDeviceCardsByDeviceIdAsync(id);
            if (activeCards.Any()) 
            {
                return Result.Failure<bool>(ErrorHelper.Validation($"Device with ID '{id}' has active device cards and cannot be deleted."));
            }

            _unitOfWork.Devices.Remove(device); // Handles soft delete via GenericRepository
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }

        public async Task<Result<DeviceDto>> LinkDeviceToLocationAsync(long id, LinkLocationToDeviceDto linkLocationDto)
        {
            _logger.LogInformation("Linking device ID: {DeviceId} to Location ID: {LocationId}", id, linkLocationDto.LocationId);
            var device = await _unitOfWork.Devices.GetByIdAsync(id);
            if (device == null || device.IsRemoved) return Result.Failure<DeviceDto>(ErrorHelper.NotFound("Device not found."));

            var location = await _unitOfWork.Locations.GetByIdAsync(linkLocationDto.LocationId);
            if (location == null || location.IsRemoved) 
                return Result.Failure<DeviceDto>(ErrorHelper.Validation("Location not found or is removed."));

            device.LocationId = linkLocationDto.LocationId;
            
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<DeviceDto>(device));
        }

        public async Task<Result<bool>> DisableDeviceAsync(long id)
        {
            _logger.LogInformation("Disabling device ID: {DeviceId}", id);
            var device = await _unitOfWork.Devices.GetByIdAsync(id); // GetByIdAsync in DeviceRepository checks IsRemoved, but not specifically IsDisabled for this action's context
            if (device == null || device.IsRemoved) // If GetById already filters IsDisabled, this check is simpler
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"Device with ID '{id}' not found or already removed."));
            }
            if (device.IsDisabled)
            {
                return Result.Failure<bool>(ErrorHelper.Conflict($"Device with ID '{id}' is already disabled."));
            }

            // The DbContext SaveChanges override will handle setting IsDisabled, DisabledAt, DisabledBy
            device.IsDisabled = true; 
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }

        public async Task<Result<bool>> EnableDeviceAsync(long id)
        {
            _logger.LogInformation("Enabling device ID: {DeviceId}", id);
            // Need to fetch even if disabled to enable it.
            var device = await _unitOfWork.Devices.FirstOrDefaultAsync(d => d.Id == id && !d.IsRemoved);
            if (device == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"Device with ID '{id}' not found or has been removed."));
            }
            if (!device.IsDisabled)
            {
                return Result.Failure<bool>(ErrorHelper.Conflict($"Device with ID '{id}' is already enabled."));
            }
            
            // The DbContext SaveChanges override will handle clearing IsDisabled, DisabledAt, DisabledBy
            device.IsDisabled = false;
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }
    }
} 