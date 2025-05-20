using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface IDeviceCardRepository : IGenericRepository<DeviceCard>
    {
        Task<PagedResult<DeviceCard>> GetAllByDeviceIdAsync(long deviceId, QueryParametersDto queryParameters, string? includeProperties = null);
        Task<DeviceCard?> GetDeviceCardBySerialNumberAsync(string serialNumber, string? includeProperties = null);
        Task<bool> IsCardSerialUniqueAsync(string cardSerial, long? currentCardId = null);
        Task<DeviceCard?> GetByIdAndDeviceIdAsync(long cardId, long deviceId, string? includeProperties = null);
        Task<IEnumerable<DeviceCard>> GetDeviceCardsByDeviceIdAsync(long deviceId, string? includeProperties = null);
        // Add other DeviceCard-specific methods, e.g., including related operations, events
    }
} 