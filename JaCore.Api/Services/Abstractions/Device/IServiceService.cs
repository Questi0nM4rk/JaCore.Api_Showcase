using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IServiceService // For ServiceEntity
    {
        Task<Result<PagedResult<ServiceDto>>> GetAllServicesAsync(QueryParametersDto queryParameters);
        Task<Result<ServiceDto>> GetServiceByIdAsync(int id, string? include = null);
        Task<Result<ServiceDto>> CreateServiceAsync(CreateServiceDto createDto);
        Task<Result<ServiceDto>> UpdateServiceAsync(int id, UpdateServiceDto updateDto);
        Task<Result<bool>> DeleteServiceAsync(int id);
        Task<Result<ServiceDto>> PatchServiceAsync(int id, PatchServiceDto patchDto);
    }
} 