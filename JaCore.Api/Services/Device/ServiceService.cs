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
using Microsoft.EntityFrameworkCore;

namespace JaCore.Api.Services.Device
{
    public class ServiceService : IServiceEntityService // Corrected interface name
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ServiceService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<ServiceDto>>> GetAllServicesAsync(QueryParametersDto queryParameters, string? includeProperties = null) // Added includeProperties
        {
            // ServiceEntities repository is IServiceEntityRepository
            var pagedResult = await _unitOfWork.ServiceEntities.GetAllAsync(queryParameters); // includeProperties not used here yet
            var serviceDtos = _mapper.Map<List<ServiceDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<ServiceDto>(
                serviceDtos,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<ServiceDto>> GetServiceByIdAsync(long id, string? include = null) // Changed long to int
        {
            // Assuming IGenericRepository.GetByIdAsync takes int and includeProperties
            // This will use ServiceEntityRepository's GetByIdAsync which in turn uses GenericRepository's.
            // CRITICAL: GenericRepository.GetByIdAsync MUST filter IsRemoved/IsDisabled or ServiceEntityRepository must override it to do so.
            var item = await _unitOfWork.ServiceEntities.GetByIdAsync(id, includeProperties: include);
            if (item == null || item.IsRemoved) // item could be non-null but IsRemoved=true if GenericRepo doesn't filter
            {
                return Result.Failure<ServiceDto>(ErrorHelper.NotFound($"Service with ID {id} not found."));
            }
            // Additional check if repository doesn't filter
            if (item.IsRemoved) {
                 return Result.Failure<ServiceDto>(ErrorHelper.NotFound($"Service with ID {id} not found."));
            }
            return Result.Success(_mapper.Map<ServiceDto>(item));
        }

        public async Task<Result<ServiceDto>> CreateServiceAsync(CreateServiceDto createDto)
        {
            if (await _unitOfWork.ServiceEntities.ExistsAsync(s => s.Name == createDto.Name && !s.IsRemoved))
            {
                return Result.Failure<ServiceDto>(ErrorHelper.Conflict($"A service with the name '{createDto.Name}' already exists."));
            }

            var newItem = _mapper.Map<ServiceEntity>(createDto);
            await _unitOfWork.ServiceEntities.AddAsync(newItem);
            await _unitOfWork.CompleteAsync(); // Changed from CommitAsync
            return Result.Success(_mapper.Map<ServiceDto>(newItem));
        }

        public async Task<Result<ServiceDto>> UpdateServiceAsync(long id, UpdateServiceDto updateDto) // Changed long to int, return ServiceDto
        {
            var existingService = await _unitOfWork.ServiceEntities.GetByIdAsync(id);
            if (existingService == null || existingService.IsRemoved) // ServiceEntity uses IsRemoved
            {
                return Result.Failure<ServiceDto>(ErrorHelper.NotFound("Service not found.", $"Service with ID '{id}' not found."));
            }

            if (existingService.Name != updateDto.Name &&
                await _unitOfWork.ServiceEntities.ExistsAsync(s => s.Name == updateDto.Name && s.Id != id && !s.IsRemoved))
            {
                return Result.Failure<ServiceDto>(ErrorHelper.Conflict($"Another service with the name '{updateDto.Name}' already exists."));
            }

            _mapper.Map(updateDto, existingService);
            _unitOfWork.ServiceEntities.Update(existingService);
            await _unitOfWork.CompleteAsync(); // Changed from CommitAsync
            return Result.Success(_mapper.Map<ServiceDto>(existingService));
        }

        public async Task<Result<bool>> DeleteServiceAsync(long id) // Changed long to int, return bool
        {
            var serviceToDelete = await _unitOfWork.ServiceEntities.GetByIdAsync(id);
            if (serviceToDelete == null || serviceToDelete.IsRemoved)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound("Service not found to delete.", $"Service with ID '{id}' not found."));
            }

            // Check if any DeviceCard is using this Service
            bool isServiceInUse = await _unitOfWork.DeviceCards.ExistsAsync(dc => dc.ServiceId == serviceToDelete.Id && !dc.IsRemoved);
            if (isServiceInUse)
            {
                string detailedMessage = $"Service '{serviceToDelete.Name}' (ID: {serviceToDelete.Id}) cannot be deleted as it is currently linked to one or more active device cards. Reason: Service is in use.";
                return Result.Failure<bool>(ErrorHelper.Conflict(detailedMessage));
            }

            _unitOfWork.ServiceEntities.Remove(serviceToDelete); // This should handle soft delete
            await _unitOfWork.CompleteAsync(); // Changed from CommitAsync
            return Result.Success(true);
        }

        public async Task<Result<ServiceDto>> PatchServiceAsync(long id, PatchServiceDto patchDto) // Added method, changed id to long
        {
            var serviceEntity = await _unitOfWork.ServiceEntities.GetByIdAsync(id);
            if (serviceEntity == null || serviceEntity.IsRemoved)
            {
                return Result.Failure<ServiceDto>(ErrorHelper.NotFound($"Service with ID {id} not found."));
            }

            // Store original name if it's part of the DTO and might be patched
            string? originalName = serviceEntity.Name;

            // Apply the patch using AutoMapper first
            _mapper.Map(patchDto, serviceEntity);

            // Check for duplicate name if name was patched and changed
            if (patchDto.Name != null && originalName != serviceEntity.Name) // Check if patchDto.Name was provided and it resulted in a change
            {
                if (await _unitOfWork.ServiceEntities.ExistsAsync(s => s.Name == serviceEntity.Name && s.Id != id && !s.IsRemoved))
                {
                    return Result.Failure<ServiceDto>(ErrorHelper.Conflict($"Another service with the name '{serviceEntity.Name}' already exists."));
                }
            }

            _unitOfWork.ServiceEntities.Update(serviceEntity);
            try
            {
                await _unitOfWork.CompleteAsync();
                return Result.Success(_mapper.Map<ServiceDto>(serviceEntity));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while patching Service with ID {ServiceId}", id);
                return Result.Failure<ServiceDto>(ErrorHelper.Conflict("The service was modified by another user. Please try again."));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error patching Service with ID {ServiceId}", id);
                return Result.Failure<ServiceDto>(ErrorHelper.ProcessFailure($"An error occurred while patching the service: {ex.Message}", "PATCH_ERROR"));
            }
        }
    }
} 