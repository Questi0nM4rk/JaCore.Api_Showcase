using JaCore.Api.Data;
using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Entities.Interfaces; // For ISoftDeletable, IDisableable
using JaCore.Api.Extensions;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions.Device;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace JaCore.Api.Repositories.Device
{
    public class ServiceEntityRepository : GenericRepository<ServiceEntity>, IServiceEntityRepository
    {
        private readonly string[] _searchableProperties = { "Name" };

        public ServiceEntityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<ServiceEntity>> GetAllAsync(QueryParametersDto queryParameters)
        {
            IQueryable<ServiceEntity> query = _dbSet;

            // Apply global filters for soft-delete
            // ServiceEntity implements ISoftDeletable
            query = query.Where(s => !s.IsRemoved);

            query = query
                .ApplySearch(queryParameters.SearchQuery, _searchableProperties)
                .ApplySort(queryParameters.SortBy)
                .IncludeProperties(queryParameters.Include);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .ToListAsync();

            return new PagedResult<ServiceEntity>(items, totalItems, queryParameters.PageNumber, queryParameters.PageSize);
        }

        public override async Task<ServiceEntity?> GetByIdAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            return (entity != null && !entity.IsRemoved) ? entity : null;
        }

        public override async Task<IEnumerable<ServiceEntity>> FindAsync(Expression<Func<ServiceEntity, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<ServiceEntity> query = _dbSet.Where(predicate).Where(s => !s.IsRemoved);
            query = query.IncludeProperties(includeProperties);
            return await query.ToListAsync();
        }

        public override async Task<bool> ExistsAsync(Expression<Func<ServiceEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).AnyAsync(s => !s.IsRemoved);
        }
    }
} 