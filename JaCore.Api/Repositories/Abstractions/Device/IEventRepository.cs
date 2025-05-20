using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<PagedResult<Event>> GetAllByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null);
        Task<Event?> GetByIdAndDeviceCardIdAsync(long eventId, long deviceCardId, string? includeProperties = null);
        Task<IEnumerable<Event>> GetEventsByDeviceCardIdAsync(long deviceCardId);
        Task<IEnumerable<Event>> GetEventsByEventTypeAsync(int eventType);
        // Add other Event-specific methods
    }
} 