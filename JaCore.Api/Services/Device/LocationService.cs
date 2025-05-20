using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.Extensions.Logging; // For logging
using System.Threading.Tasks;
using JaCore.Api.Helpers; // Added for ErrorHelper
using System.Collections.Generic; // Added for IEnumerable

namespace JaCore.Api.Services.Device
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LocationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<LocationDto>>> GetAllLocationsAsync(QueryParametersDto queryParameters)
        {
            _logger.LogInformation("Fetching all locations with query parameters: {@QueryParameters}", queryParameters);
            var pagedResult = await _unitOfWork.Locations.GetAllAsync(queryParameters);
            var locationDtos = _mapper.Map<IEnumerable<LocationDto>>(pagedResult.Items);
            var pagedLocationDtos = new PagedResult<LocationDto>(locationDtos, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
            return Result.Success(pagedLocationDtos);
        }

        public async Task<Result<LocationDto>> GetLocationByIdAsync(long id, string? include)
        {
            _logger.LogInformation("Fetching location by ID {LocationId} with includes: {Include}", id, include);
            var location = await _unitOfWork.Locations.GetByIdAsync(id, include);
            if (location == null || location.IsRemoved) 
            {
                return Result.Failure<LocationDto>(ErrorHelper.NotFound($"Location with ID {id} not found."));
            }
            return Result.Success(_mapper.Map<LocationDto>(location));
        }

        public async Task<Result<LocationDto>> GetLocationByNameAsync(string name, string? include)
        {
            _logger.LogInformation("Fetching location by Name {LocationName} with includes: {Include}", name, include);
            var location = await _unitOfWork.Locations.GetLocationByNameAsync(name, include);
            if (location == null) 
            {
                return Result.Failure<LocationDto>(ErrorHelper.NotFound($"Location with name '{name}' not found."));
            }
            return Result.Success(_mapper.Map<LocationDto>(location));
        }

        public async Task<Result<LocationDto>> CreateLocationAsync(CreateLocationDto createLocationDto)
        {
            var existingLocation = await _unitOfWork.Locations.GetLocationByNameAsync(createLocationDto.Name);
            if (existingLocation != null)
            {
                return Result.Failure<LocationDto>(ErrorHelper.Conflict($"A location with the name '{createLocationDto.Name}' already exists."));
            }

            var location = _mapper.Map<Location>(createLocationDto);
            await _unitOfWork.Locations.AddAsync(location);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<LocationDto>(location));
        }

        public async Task<Result<LocationDto>> UpdateLocationAsync(long id, UpdateLocationDto updateLocationDto)
        {
            var location = await _unitOfWork.Locations.GetByIdAsync(id);
            if (location == null || location.IsRemoved)
            {
                return Result.Failure<LocationDto>(ErrorHelper.NotFound($"Location with ID {id} not found."));
            }
            if (location.Name != updateLocationDto.Name && !await _unitOfWork.Locations.IsNameUniqueAsync(updateLocationDto.Name, location.Id))
            {
                return Result.Failure<LocationDto>(ErrorHelper.Conflict($"A location with the name '{updateLocationDto.Name}' already exists."));
            }
            _mapper.Map(updateLocationDto, location);
            _unitOfWork.Locations.Update(location);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<LocationDto>(location));
        }

        public async Task<Result<bool>> DeleteLocationAsync(long id)
        {
            var location = await _unitOfWork.Locations.GetByIdAsync(id);
            if (location == null || location.IsRemoved)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"Location with ID {id} not found or already deleted."));
            }
            // Check dependencies: Devices
            // This requires ILocationRepository to have a method like HasAssociatedDevicesAsync(int locationId)
            // or direct query here. For now, assume no direct check or it's handled elsewhere.
            _unitOfWork.Locations.Remove(location);
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }

        public async Task<Result<LocationDto>> PatchLocationAsync(long id, PatchLocationDto patchLocationDto)
        {
            var location = await _unitOfWork.Locations.GetByIdAsync(id);
            if (location == null || location.IsRemoved)
            {
                return Result.Failure<LocationDto>(ErrorHelper.NotFound($"Location with ID {id} not found."));
            }
            if (patchLocationDto.Name != null && patchLocationDto.Name != location.Name && !await _unitOfWork.Locations.IsNameUniqueAsync(patchLocationDto.Name, location.Id))
            {
                return Result.Failure<LocationDto>(ErrorHelper.Conflict($"A location with the name '{patchLocationDto.Name}' already exists."));
            }
            if (patchLocationDto.Name != null) location.Name = patchLocationDto.Name;
            _unitOfWork.Locations.Update(location);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<LocationDto>(location));
        }
    }
} 