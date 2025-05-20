using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IDeviceOperationService
    {
        Task<Result<PagedResult<DeviceOperationDto>>> GetDeviceOperationsByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null);
        Task<Result<DeviceOperationDto>> GetDeviceOperationByIdAsync(long id, string? includeProperties = null);
        Task<Result<DeviceOperationDto>> GetDeviceOperationByIdAndDeviceCardIdAsync(long operationId, long deviceCardId, string? includeProperties = null);
        Task<Result<DeviceOperationDto>> CreateDeviceOperationAsync(long deviceCardId, CreateDeviceOperationDto createDeviceOperationDto);
        Task<Result<DeviceOperationDto>> UpdateDeviceOperationAsync(long deviceCardId, long operationId, UpdateDeviceOperationDto updateDeviceOperationDto);
        Task<Result<DeviceOperationDto>> PatchDeviceOperationAsync(long deviceCardId, long operationId, PatchDeviceOperationDto patchDeviceOperationDto);
        Task<Result<bool>> DeleteDeviceOperationAsync(long operationId, long deviceCardId);
        Task<Result<IEnumerable<DeviceOperationDto>>> UpdateDeviceOperationsOrderAsync(long deviceCardId, List<OrderedOperationDto> orderedOperationsDto);
    }
} 