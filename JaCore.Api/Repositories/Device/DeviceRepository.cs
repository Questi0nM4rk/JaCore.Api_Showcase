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
using DeviceEntity = JaCore.Api.Entities.Device.Device;

namespace JaCore.Api.Repositories.Device
{
    public class DeviceRepository : GenericRepository<DeviceEntity>, IDeviceRepository
    {
        private readonly string[] _searchableProperties = { "Id", "Name", "LocationId", "IsDisabled" };

        public DeviceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<DeviceEntity>> GetAllAsync(QueryParametersDto queryParameters, long? locationId = null, string? includeProperties = null)
        {
            IQueryable<DeviceEntity> query = _dbSet.Where(d => !d.IsRemoved && !d.IsDisabled);

            if (locationId.HasValue)
            {
                query = query.Where(d => d.LocationId == locationId.Value);
            }

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
            return new PagedResult<DeviceEntity>(items, queryParameters.PageNumber, queryParameters.PageSize, totalItems);
        }

        public async Task<DeviceEntity?> GetDeviceByNameAsync(string name, string? includeProperties = null)
        {
             IQueryable<DeviceEntity> query = _dbSet
                .Where(d => d.Name == name && !d.IsRemoved && !d.IsDisabled);

            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DeviceEntity>> GetDevicesByLocationIDAsync(long locationId, string? includeProperties = null)
        {
            IQueryable<DeviceEntity> query = _dbSet
                .Where(d => d.LocationId == locationId && !d.IsRemoved && !d.IsDisabled);
            
            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, string? currentDeviceId = null)
        {
            var query = _dbSet.Where(d => d.Name == name && !d.IsRemoved);
            if (!string.IsNullOrEmpty(currentDeviceId))
            {
                if (long.TryParse(currentDeviceId, out long id))
                {
                    query = query.Where(d => d.Id != id);
                }
                else
                {
                    // Potentially log or handle invalid currentDeviceId string that's not a long
                    // For now, if it's not a valid long, we assume it's not a match to any existing ID.
                }
            }
            return !await query.AnyAsync();
        }

        // Override GetByIdAsync to ensure IsRemoved and IsDisabled are checked, as Device implements both
        public override async Task<DeviceEntity?> GetByIdAsync(object id)
        {
            var device = await base.GetByIdAsync(id);
            if (device != null && (device.IsRemoved || device.IsDisabled))
            {
                return null;
            }
            return device;
        }

        // Override GetAllAsync from GenericRepository to also filter by IsDisabled for Device entities
        public override async Task<IEnumerable<DeviceEntity>> GetAllAsync()
        {
            return await _dbSet.Where(d => !d.IsRemoved && !d.IsDisabled).AsNoTracking().ToListAsync();
        }
    }
} 