using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using JaCore.Api.Helpers;

namespace JaCore.Api.Services.Device
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EventService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<EventDto>>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            return Result.Success(_mapper.Map<IEnumerable<EventDto>>(events));
        }

        public async Task<Result<PagedResult<EventDto>>> GetEventsByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching events for DeviceCardId: {DeviceCardId}, Query: {@QueryParameters}", deviceCardId, queryParameters);
            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(deviceCardId);
            if (deviceCard == null)
            {
                return Result.Failure<PagedResult<EventDto>>(ErrorHelper.NotFound($"DeviceCard with ID '{deviceCardId}' not found."));
            }

            var pagedResult = await _unitOfWork.Events.GetAllByDeviceCardIdAsync(deviceCardId, queryParameters, includeProperties);
            var eventDtos = _mapper.Map<List<EventDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<EventDto>(eventDtos, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<EventDto>> GetEventByIdAsync(long id, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching event by ID: {EventId}, Include: {Include}", id, includeProperties);
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id, includeProperties);
            if (eventEntity == null)
            {
                return Result.Failure<EventDto>(ErrorHelper.NotFound($"Event with ID '{id}' not found."));
            }
            return Result.Success(_mapper.Map<EventDto>(eventEntity));
        }

        public async Task<Result<EventDto>> GetEventByIdAndDeviceCardIdAsync(long eventId, long deviceCardId, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching event by EventID: {EventId} and DeviceCardID: {DeviceCardId}, Include: {Include}", eventId, deviceCardId, includeProperties);
            var eventEntity = await _unitOfWork.Events.GetByIdAndDeviceCardIdAsync(eventId, deviceCardId, includeProperties);
            if (eventEntity == null)
            {
                return Result.Failure<EventDto>(ErrorHelper.NotFound($"Event with ID '{eventId}' associated with DeviceCard ID '{deviceCardId}' not found."));
            }
            return Result.Success(_mapper.Map<EventDto>(eventEntity));
        }

        public async Task<Result<EventDto>> CreateEventAsync(CreateEventDto createEventDto)
        {
            _logger.LogInformation("Creating new event: {@CreateEventDto}", createEventDto);

            var deviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(createEventDto.DeviceCardId);
            if (deviceCard == null || deviceCard.IsRemoved)
            {
                return Result.Failure<EventDto>(ErrorHelper.Validation("DeviceCard not found or is removed.", 
                    $"DeviceCard with ID '{createEventDto.DeviceCardId}' not found or is removed, cannot create event."));
            }
            
            var eventEntity = _mapper.Map<Event>(createEventDto);
            eventEntity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Events.AddAsync(eventEntity);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<EventDto>(eventEntity));
        }

        public async Task<Result<bool>> DeleteEventAsync(long id)
        {
            _logger.LogInformation("Attempting to delete event ID: {EventId}", id);
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"Event with ID '{id}' not found."));
            }

            _unitOfWork.Events.Remove(eventEntity);
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }

        public async Task<Result<EventDto>> PatchEventAsync(long id, PatchEventDto patchEventDto)
        {
            _logger.LogInformation("Patching event ID: {EventId} with DTO: {@PatchEventDto}", id, patchEventDto);
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return Result.Failure<EventDto>(ErrorHelper.NotFound($"Event with ID '{id}' not found."));
            }
            
            if (patchEventDto.DeviceCardId.HasValue && eventEntity.DeviceCardId != patchEventDto.DeviceCardId.Value)
            {
                var newDeviceCard = await _unitOfWork.DeviceCards.GetByIdAsync(patchEventDto.DeviceCardId.Value);
                if (newDeviceCard == null)
                {
                    return Result.Failure<EventDto>(ErrorHelper.Validation($"Target DeviceCard with ID '{patchEventDto.DeviceCardId.Value}' not found or is inaccessible."));
                }
            }

            _mapper.Map(patchEventDto, eventEntity);
            eventEntity.ModifiedAt = DateTime.UtcNow;
            _unitOfWork.Events.Update(eventEntity);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<EventDto>(eventEntity));
        }
    }
} 