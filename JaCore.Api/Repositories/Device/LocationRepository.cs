using JaCore.Api.Data;
using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions.Device;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using JaCore.Api.Extensions; // For IQueryableExtensions like ApplySort, IncludeProperties (to be created)

namespace JaCore.Api.Repositories.Device
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        private new readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Location?> GetLocationByNameAsync(string name, string? includeProperties = null)
        {
            IQueryable<Location> query = _context.Locations
                                 .Where(l => l.Name == name && !l.IsRemoved);
            
            query = query.IncludeProperties(includeProperties); // Use extension method

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Location>> GetAllAsync(QueryParametersDto queryParameters)
        {
            IQueryable<Location> query = _context.Locations
                                                 .Where(l => !l.IsRemoved);

            // Apply Search
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchQuery))
            {
                var searchQueryLower = queryParameters.SearchQuery.ToLower();
                query = query.Where(l => (l.Name != null && l.Name.ToLower().Contains(searchQueryLower)));
            }

            // Apply Include Properties
            query = query.IncludeProperties(queryParameters.Include);

            // Apply Sorting
            query = query.ApplySort(queryParameters.SortBy);

            var totalCount = await query.CountAsync();

            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .AsNoTracking() // Keep AsNoTracking for read operations
                                   .ToListAsync();

            return new PagedResult<Location>(items, queryParameters.PageNumber, queryParameters.PageSize, totalCount);
        }

        // Override or add other specific methods from IGenericRepository if needed for Location
        public override async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _dbSet.Where(l => !l.IsRemoved).ToListAsync();
        }

        public override async Task<Location?> GetByIdAsync(object id)
        {
            var location = await _dbSet.FindAsync(id);
            if (location != null && location.IsRemoved)
            {
                return null; // Treat soft-deleted as not found by ID
            }
            return location;
        }

        public async Task<bool> IsNameUniqueAsync(string name, long? currentId = null)
        {
            IQueryable<Location> query = _dbSet.Where(l => l.Name == name && !l.IsRemoved);
            if (currentId.HasValue)
            {
                query = query.Where(l => l.Id != currentId.Value);
            }
            return !await query.AnyAsync();
        }
    }
} 