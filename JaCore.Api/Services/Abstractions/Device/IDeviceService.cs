using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IDeviceService
    {
        Task<Result<PagedResult<DeviceDto>>> GetAllDevicesAsync(QueryParametersDto queryParameters, long? locationId = null, string? includeProperties = null);
        Task<Result<DeviceDto>> GetDeviceByIdAsync(long id, string? includeProperties = null);
        Task<Result<DeviceDto>> GetDeviceByNameAsync(string name, string? includeProperties = null);
        Task<Result<DeviceDto>> CreateDeviceAsync(CreateDeviceDto createDeviceDto);
        Task<Result<DeviceDto>> UpdateDeviceAsync(long id, UpdateDeviceDto updateDeviceDto);
        Task<Result<DeviceDto>> PatchDeviceAsync(long id, PatchDeviceDto patchDeviceDto);
        Task<Result<bool>> DeleteDeviceAsync(long id); // Soft delete
        Task<Result<DeviceDto>> LinkDeviceToLocationAsync(long id, LinkLocationToDeviceDto linkLocationDto);
        Task<Result<bool>> DisableDeviceAsync(long id);
        Task<Result<bool>> EnableDeviceAsync(long id);
    }
} 