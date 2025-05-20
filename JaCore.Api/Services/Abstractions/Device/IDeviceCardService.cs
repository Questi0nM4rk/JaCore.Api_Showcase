using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IDeviceCardService
    {
        Task<Result<PagedResult<DeviceCardDto>>> GetDeviceCardsByDeviceIdAsync(long deviceId, QueryParametersDto queryParameters, string? includeProperties = null);
        Task<Result<DeviceCardDto>> GetDeviceCardByIdAsync(long id, string? includeProperties = null);
        Task<Result<DeviceCardDto>> GetDeviceCardByIdAndDeviceIdAsync(long cardId, long deviceId, string? includeProperties = null);
        Task<Result<DeviceCardDto>> GetDeviceCardBySerialNumberAsync(string serialNumber, string? includeProperties = null);
        Task<Result<DeviceCardDto>> CreateDeviceCardAsync(CreateDeviceCardDto createDeviceCardDto);
        Task<Result<DeviceCardDto>> UpdateDeviceCardAsync(long id, UpdateDeviceCardDto updateDeviceCardDto);
        Task<Result<DeviceCardDto>> PatchDeviceCardAsync(long id, PatchDeviceCardDto patchDeviceCardDto);
        Task<Result<bool>> DeleteDeviceCardAsync(long id); // Soft delete
    }
} 