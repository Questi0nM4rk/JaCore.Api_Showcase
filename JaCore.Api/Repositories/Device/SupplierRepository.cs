using JaCore.Api.Data;
using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Extensions;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions.Device;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Device
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        private readonly string[] _searchableProperties = { "Name", "ContactName", "ContactEmail", "Address" };

        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Supplier>> GetAllAsync(QueryParametersDto queryParameters, string? includeProperties = null)
        {
            IQueryable<Supplier> query = _dbSet.Where(s => !s.IsRemoved); // Filter out soft-deleted

            // Apply Search (using a specific implementation for Supplier if different from a generic one)
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
            return new PagedResult<Supplier>(items, queryParameters.PageNumber, queryParameters.PageSize, totalItems);
        }

        public async Task<Supplier?> GetSupplierByNameAsync(string name, string? includeProperties = null)
        {
            IQueryable<Supplier> query = _dbSet
                .Where(s => s.Name == name && !s.IsRemoved);
            
            query = query.IncludeProperties(includeProperties);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, long? currentId = null)
        {
            var query = _dbSet.Where(s => s.Name == name && !s.IsRemoved);
            if (currentId.HasValue)
            {
                query = query.Where(s => s.Id != currentId.Value);
            }
            return !await query.AnyAsync();
        }
        
        // Override GenericRepository methods if specific filtering (e.g., !IsDisabled) is always needed for Suppliers
        public override async Task<Supplier?> GetByIdAsync(object id)
        {
            var supplier = await base.GetByIdAsync(id);
            if (supplier != null && supplier.IsRemoved) // Suppliers also have IsRemoved
            {
                return null; 
            }
            return supplier;
        }

        public override async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            // Suppliers only implement ISoftDeletable, not IDisableable
            return await _dbSet.Where(s => !s.IsRemoved).AsNoTracking().ToListAsync();
        }
    }
} 