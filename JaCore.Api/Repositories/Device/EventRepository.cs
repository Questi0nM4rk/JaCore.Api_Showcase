using JaCore.Api.Data;
using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Extensions;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions.Device;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Device
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        private readonly string[] _searchableProperties = { "EventType", "Description" }; // Assuming EventType is string or converted

        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Event>> GetAllByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null)
        {
            IQueryable<Event> query = _dbSet
                .Where(e => e.DeviceCardId == deviceCardId && !e.IsRemoved);

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchQuery))
            {
                // Assuming EventType is an int or enum, searching might require specific handling.
                // If EventType is an enum, convert SearchQuery to the enum type if possible,
                // or search Description only if SearchQuery isn't an integer.
                query = query.ApplySearch(queryParameters.SearchQuery, _searchableProperties);
            }

            query = query.IncludeProperties(includeProperties);
            query = query.ApplySort(queryParameters.SortBy);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();
            return new PagedResult<Event>(items, queryParameters.PageNumber, queryParameters.PageSize, totalItems);
        }

        public async Task<Event?> GetByIdAndDeviceCardIdAsync(long eventId, long deviceCardId, string? includeProperties = null)
        {
            IQueryable<Event> query = _dbSet
                .Where(e => e.Id == eventId && e.DeviceCardId == deviceCardId && !e.IsRemoved);

            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDeviceCardIdAsync(long deviceCardId)
        {
            return await _dbSet
                .Where(e => e.DeviceCardId == deviceCardId && !e.IsRemoved)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByEventTypeAsync(int eventType)
        {
            // Assuming EventType on the entity is an int or matches an int representation
            return await _dbSet
                .Where(e => e.EventType == eventType && !e.IsRemoved)
                .AsNoTracking()
                .ToListAsync();
        }
        
        // Event only implements ISoftDeletable, not IDisableable.
        // Override GenericRepository methods if specific filtering is always needed.
        public override async Task<Event?> GetByIdAsync(object id)
        {
            var ev = await base.GetByIdAsync(id);
            if (ev != null && ev.IsRemoved)
            {
                return null;
            }
            return ev;
        }

        public override async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsRemoved).AsNoTracking().ToListAsync();
        }
    }
} 