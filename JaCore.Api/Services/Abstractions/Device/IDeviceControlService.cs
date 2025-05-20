using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IDeviceControlService // For Device entity
    {
        Task<Result<IEnumerable<DeviceDto>>> GetAllDevicesAsync();
        Task<Result<DeviceDto>> GetDeviceByIdAsync(long id);
        Task<Result<DeviceDto>> GetDeviceByNameAsync(string name);
        Task<Result<IEnumerable<DeviceDto>>> GetDevicesByLocationIdAsync(long locationId);
        Task<Result<DeviceDto>> CreateDeviceAsync(CreateDeviceDto createDto);
        Task<Result<DeviceDto>> UpdateDeviceAsync(long id, UpdateDeviceDto updateDto);
        Task<Result<bool>> DeleteDeviceAsync(long id);
        Task<Result<DeviceDto>> DisableDeviceAsync(long id);
        Task<Result<DeviceDto>> EnableDeviceAsync(long id);
        Task<Result<DeviceDto>> LinkDeviceLocationAsync(long deviceId, LinkLocationToDeviceDto linkLocationDto);
        Task<Result<DeviceDto>> PatchDeviceAsync(long id, PatchDeviceDto patchDeviceDto);
    }
} 