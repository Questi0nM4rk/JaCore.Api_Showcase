using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    // Path: api/v1.0/device/{deviceId:long}/cards/{cardId:long}/events
    [Route(ApiConstants.BasePaths.EventCollection)]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet(ApiConstants.EventEndpoints.GetAll)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetEventsForDeviceCard(long deviceId, long cardId, [FromQuery] QueryParametersDto queryParams)
        {
            // Ensure cardId from route is used, not something from queryParams if it existed there
            var result = await _eventService.GetEventsByDeviceCardIdAsync(cardId, queryParams);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.EventId)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetEventById(long deviceId, long cardId, long eventId)
        {
            var result = await _eventService.GetEventByIdAndDeviceCardIdAsync(eventId, cardId);
            return result.ToActionResult(this);
        }

        [HttpPost(ApiConstants.EventEndpoints.Create)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> CreateEventForDeviceCard(long deviceId, long cardId, [FromBody] CreateEventDto createDto)
        {
            // The CreateEventDto has DeviceCardId. We should ensure it matches the cardId from the route.
            // The service will use createDto.DeviceCardId.
            if (createDto.DeviceCardId == 0) // If DTO allows it to be 0 for auto-assignment
            {
                createDto.DeviceCardId = cardId; 
            }
            else if (createDto.DeviceCardId != cardId) 
            {
                // Use ErrorHelper for creating validation error responses
                return BadRequest(ErrorHelper.Validation("Mismatched DeviceCardId", "DeviceCardId in body does not match cardId from route."));
            }

            var result = await _eventService.CreateEventAsync(createDto);
            if (!result.IsSuccess) return result.ToActionResult(this);
            return CreatedAtAction(nameof(GetEventById), new { deviceId = deviceId, cardId = cardId, eventId = result.Value!.Id }, result.Value);
        }
        
        // Events are generally immutable. No PUT, PATCH, DELETE for individual events through this controller normally.
        // If deletion is required, it might be a bulk operation or an admin-only feature via a different mechanism or service call.
    }
} 