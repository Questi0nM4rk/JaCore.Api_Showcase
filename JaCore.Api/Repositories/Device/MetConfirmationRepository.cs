using JaCore.Api.Data;
using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Extensions; // For IQueryableExtensions
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions.Device;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Device
{
    public class MetConfirmationRepository : GenericRepository<MetConfirmation>, IMetConfirmationRepository
    {
        private new readonly ApplicationDbContext _context;
        private readonly string[] _searchableProperties = { "Name", "Description" };

        public MetConfirmationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<MetConfirmation>> GetAllAsync(QueryParametersDto queryParameters)
        {
            IQueryable<MetConfirmation> query = _dbSet; // _dbSet from GenericRepository is already filtered for IsRemoved and IsDisabled

            query = query
                .ApplySearch(queryParameters.SearchQuery, _searchableProperties)
                .ApplySort(queryParameters.SortBy)
                .IncludeProperties(queryParameters.Include);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .ToListAsync();

            return new PagedResult<MetConfirmation>(items, totalItems, queryParameters.PageNumber, queryParameters.PageSize);
        }

        public async Task<bool> IsNameUniqueAsync(string name, long? currentId = null)
        {
            IQueryable<MetConfirmation> query = _dbSet.Where(mc => mc.Name == name && !mc.IsRemoved);
            if (currentId.HasValue)
            {
                query = query.Where(mc => mc.Id != currentId.Value);
            }
            return !await query.AnyAsync();
        }

        /* Method seems to have incorrect logic (MetConfirmation has no DeviceId or ConfirmationText)
           and is not present in IMetConfirmationRepository. Commenting out.
        public async Task<PagedResult<MetConfirmation>> GetByDeviceIdAsync(long deviceId, QueryParametersDto queryParameters)
        {
            IQueryable<MetConfirmation> query = _context.MetConfirmations
                                                        .Where(mc => mc.DeviceId == deviceId && !mc.IsRemoved); // mc.IsDisabled was also here

            // Search within this device's confirmations
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchQuery))
            {
                var sq = queryParameters.SearchQuery.ToLower();
                query = query.Where(mc => mc.ConfirmationText != null && mc.ConfirmationText.ToLower().Contains(sq));
            }
            
            query = query.IncludeProperties(queryParameters.Include);
            query = query.ApplySort(queryParameters.SortBy);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();
            return new PagedResult<MetConfirmation>(items, queryParameters.PageNumber, queryParameters.PageSize, totalCount);
        }
        */
    }
} 