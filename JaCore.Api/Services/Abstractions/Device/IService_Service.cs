using JaCore.Api.DTOs.Device; // Assuming PatchServiceDto is here
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IService_Service // Renamed to avoid conflict, matching ServiceService.cs
    {
        Task<Result<IEnumerable<ServiceDto>>> GetAllServicesAsync();
        Task<Result<ServiceDto>> GetServiceByIdAsync(int id);
        Task<Result<ServiceDto>> CreateServiceAsync(CreateServiceDto createServiceDto);
        Task<Result<ServiceDto>> UpdateServiceAsync(int id, UpdateServiceDto updateServiceDto);
        Task<Result<bool>> DeleteServiceAsync(int id);
        Task<Result<ServiceDto>> PatchServiceAsync(int id, PatchServiceDto patchServiceDto);
    }
} 