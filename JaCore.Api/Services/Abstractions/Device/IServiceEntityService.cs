using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device; // Assuming ServiceDto, CreateServiceDto etc. are here
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device;

public interface IServiceEntityService 
{
    Task<Result<PagedResult<ServiceDto>>> GetAllServicesAsync(QueryParametersDto queryParameters, string? includeProperties = null);
    Task<Result<ServiceDto>> GetServiceByIdAsync(long id, string? includeProperties = null);
    Task<Result<ServiceDto>> CreateServiceAsync(CreateServiceDto createDto);
    Task<Result<ServiceDto>> UpdateServiceAsync(long id, UpdateServiceDto updateDto);
    Task<Result<bool>> DeleteServiceAsync(long id);
    Task<Result<ServiceDto>> PatchServiceAsync(long id, PatchServiceDto patchDto);
} 