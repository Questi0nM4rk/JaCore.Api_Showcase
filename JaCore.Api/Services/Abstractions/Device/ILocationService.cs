using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface ILocationService
    {
        Task<Result<PagedResult<LocationDto>>> GetAllLocationsAsync(QueryParametersDto queryParameters);
        Task<Result<LocationDto>> GetLocationByIdAsync(long id, string? include);
        Task<Result<LocationDto>> GetLocationByNameAsync(string name, string? include);
        Task<Result<LocationDto>> CreateLocationAsync(CreateLocationDto createLocationDto);
        Task<Result<LocationDto>> UpdateLocationAsync(long id, UpdateLocationDto updateLocationDto);
        Task<Result<bool>> DeleteLocationAsync(long id); // Soft delete
        Task<Result<LocationDto>> PatchLocationAsync(long id, PatchLocationDto patchLocationDto);
    }
} 