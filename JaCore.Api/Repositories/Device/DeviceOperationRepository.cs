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
using System.Linq.Expressions;
using System.Threading;

namespace JaCore.Api.Repositories.Device
{
    public class DeviceOperationRepository : GenericRepository<DeviceOperation>, IDeviceOperationRepository
    {
        // No specific searchable properties defined for DeviceOperation yet, can be added if needed.
        // private readonly string[] _searchableProperties = { "Name", "Description" }; 

        public DeviceOperationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<DeviceOperation>> GetAllByDeviceCardIdAsync(long deviceCardId, QueryParametersDto queryParameters, string? includeProperties = null)
        {
            IQueryable<DeviceOperation> query = _dbSet
                .Where(op => op.DeviceCards.Any(dc => dc.Id == deviceCardId) && !op.IsRemoved);

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchQuery))
            {
                // Add search logic if _searchableProperties are defined and ApplySearch is applicable
                // query = query.ApplySearch(queryParameters.SearchQuery, _searchableProperties);
            }

            query = query.IncludeProperties(includeProperties);
            // DeviceOperation has Order_No, could be a default sort if not specified
            if (string.IsNullOrWhiteSpace(queryParameters.SortBy))
            {
                query = query.OrderBy(op => op.OrderNo);
            }
            else
            {
                query = query.ApplySort(queryParameters.SortBy);
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();
            return new PagedResult<DeviceOperation>(items, queryParameters.PageNumber, queryParameters.PageSize, totalItems);
        }

        public async Task<DeviceOperation?> GetByIdAndDeviceCardIdAsync(long operationId, long deviceCardId, string? includeProperties = null)
        {
            IQueryable<DeviceOperation> query = _dbSet
                .Where(op => op.Id == operationId && op.DeviceCards.Any(dc => dc.Id == deviceCardId) && !op.IsRemoved);
            
            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DeviceOperation>> GetDeviceOperationsByDeviceCardIdAsync(long deviceCardId)
        {
            return await _dbSet
                .Where(op => op.DeviceCards.Any(dc => dc.Id == deviceCardId) && !op.IsRemoved)
                .OrderBy(op => op.OrderNo) // Default order by Order_No
                .AsNoTracking()
                .ToListAsync();
        }

        // DeviceOperation only implements ISoftDeletable.
        public override async Task<DeviceOperation?> GetByIdAsync(object id)
        {
            var op = await base.GetByIdAsync(id);
            if (op != null && op.IsRemoved)
            {
                return null;
            }
            return op;
        }

        public async Task<IEnumerable<DeviceOperation>> GetAllAsync(Expression<Func<DeviceOperation, bool>>? predicate = null,
                                                               CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
             query = query.Where(op => !op.IsRemoved); // Ensuring soft delete is respected

            // Eagerly load related DeviceCards
            query = query.Include(op => op.DeviceCards);
            
            // Apply default ordering if applicable
            query = query.OrderBy(op => op.OrderNo);

            return await query.ToListAsync(cancellationToken);
        }
    }
} 