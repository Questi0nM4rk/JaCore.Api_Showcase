using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IEventService
    {
        Task<Result<IEnumerable<EventDto>>> GetAllEventsAsync();
        Task<Result<PagedResult<EventDto>>> GetEventsByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null);
        Task<Result<EventDto>> GetEventByIdAsync(long id, string? includeProperties = null);
        Task<Result<EventDto>> GetEventByIdAndDeviceCardIdAsync(long eventId, long deviceCardId, string? includeProperties = null);
        Task<Result<EventDto>> CreateEventAsync(CreateEventDto createEventDto);
        Task<Result<bool>> DeleteEventAsync(long id);
        Task<Result<EventDto>> PatchEventAsync(long id, PatchEventDto patchEventDto);
    }
} 