using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface IDeviceOperationRepository : IGenericRepository<DeviceOperation>
    {
        Task<PagedResult<DeviceOperation>> GetAllByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null);
        Task<DeviceOperation?> GetByIdAndDeviceCardIdAsync(long operationId, long deviceCardId, string? includeProperties = null);
        Task<IEnumerable<DeviceOperation>> GetDeviceOperationsByDeviceCardIdAsync(long deviceCardId);
        // Add other DeviceOperation-specific methods
    }
} 