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
    public class DeviceCardRepository : GenericRepository<DeviceCard>, IDeviceCardRepository
    {
        private readonly string[] _searchableProperties = { "SerialNumber", "CardType", "Version" };

        public DeviceCardRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<DeviceCard>> GetAllByDeviceIdAsync(long deviceId, QueryParametersDto queryParameters, string? includeProperties = null)
        {
            IQueryable<DeviceCard> query = _dbSet
                .Where(dc => dc.DeviceId == deviceId && !dc.IsRemoved);

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchQuery))
            {
                query = query.ApplySearch(queryParameters.SearchQuery, _searchableProperties);
            }

            query = query.IncludeProperties(includeProperties);
            query = query.ApplySort(queryParameters.SortBy);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();
            return new PagedResult<DeviceCard>(items, queryParameters.PageNumber, queryParameters.PageSize, totalItems);
        }

        public async Task<DeviceCard?> GetDeviceCardBySerialNumberAsync(string serialNumber, string? includeProperties = null)
        {
            IQueryable<DeviceCard> query = _dbSet
                .Where(dc => dc.SerialNumber == serialNumber && !dc.IsRemoved);

            query = query.IncludeProperties(includeProperties);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DeviceCard>> GetDeviceCardsByDeviceIdAsync(long deviceId, string? includeProperties = null)
        {
            IQueryable<DeviceCard> query = _dbSet
                .Where(dc => dc.DeviceId == deviceId && !dc.IsRemoved);
            
            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<bool> IsCardSerialUniqueAsync(string serialNumber, long? currentCardId = null)
        {
            var query = _dbSet.Where(dc => dc.SerialNumber == serialNumber && !dc.IsRemoved);
            if (currentCardId.HasValue)
            {
                query = query.Where(dc => dc.Id != currentCardId.Value);
            }
            return !await query.AnyAsync();
        }
        
        public async Task<DeviceCard?> GetByIdAndDeviceIdAsync(long cardId, long deviceId, string? includeProperties = null)
        {
            IQueryable<DeviceCard> query = _dbSet
                .Where(dc => dc.Id == cardId && dc.DeviceId == deviceId && !dc.IsRemoved);
            
            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        // Override GetByIdAsync to ensure IsRemoved is checked
        public override async Task<DeviceCard?> GetByIdAsync(object id)
        {
            var card = await base.GetByIdAsync(id);
            if (card != null && (card.IsRemoved || card.IsDisabled))
            {
                return null;
            }
            return card;
        }

        // Override GetAllAsync from GenericRepository to filter by IsRemoved
        public override async Task<IEnumerable<DeviceCard>> GetAllAsync()
        {
            return await _dbSet.Where(dc => !dc.IsRemoved && !dc.IsDisabled).AsNoTracking().ToListAsync();
        }

        public async Task<DeviceCard?> GetByDeviceAndSerialNumberAsync(long deviceId, string serialNumber)
        {
            return await _context.DeviceCards
                                 .FirstOrDefaultAsync(dc => dc.DeviceId == deviceId && dc.SerialNumber == serialNumber && !dc.IsRemoved);
        }

        public async Task<bool> DoesCardExistForDeviceAsync(long deviceId)
        {
            return await _context.DeviceCards.AnyAsync(dc => dc.DeviceId == deviceId && !dc.IsRemoved);
        }

        public async Task<DeviceCard?> GetActiveCardByDeviceIdAsync(long deviceId)
        {
            return await _context.DeviceCards
                                 .FirstOrDefaultAsync(dc => dc.DeviceId == deviceId && !dc.IsRemoved);
        }

        public async Task<bool> IsSerialNumberUniqueAsync(string serialNumber, long? currentCardId = null)
        {
            var query = _dbSet.Where(dc => dc.SerialNumber == serialNumber && !dc.IsRemoved);
            if (currentCardId.HasValue)
            {
                query = query.Where(dc => dc.Id != currentCardId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<bool> HasActiveOperationsOrEventsAsync(long cardId)
        {
            bool hasActiveOps = await _context.DeviceOperations
                .Where(op => op.DeviceCards.Any(dc => dc.Id == cardId) && !op.IsRemoved)
                .AnyAsync();

            if (hasActiveOps) return true;

            bool hasActiveEvents = await _context.Events
                                         .AnyAsync(e => e.DeviceCardId == cardId && !e.IsRemoved);
            return hasActiveEvents;
        }

        public async Task<bool> DeviceHasActiveCardAsync(long deviceId, long? currentCardId = null)
        {
            var query = _dbSet.Where(dc => dc.DeviceId == deviceId && !dc.IsRemoved && !dc.IsDisabled);
            if (currentCardId.HasValue)
            {
                query = query.Where(dc => dc.Id != currentCardId.Value); // Prevent matching the card being updated
            }
            return await query.AnyAsync();
        }
    }
} 