using JaCore.Api.Data;
using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Template;
using JaCore.Api.Extensions;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions.Template;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Template
{
    public class TemplateUIElemRepository : GenericRepository<TemplateUIElem>, ITemplateUIElemRepository
    {
        private readonly string[] _searchableProperties = { "Name" };

        public TemplateUIElemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<TemplateUIElem>> GetAllAsync(QueryParametersDto queryParameters, string? includeProperties = null)
        {
            // TemplateUIElem does not implement ISoftDeletable or IDisableable as per current entity definition
            IQueryable<TemplateUIElem> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchQuery))
            {
                query = query.ApplySearch(queryParameters.SearchQuery, _searchableProperties);
            }
            
            query = query.IncludeProperties(includeProperties);
            query = query.ApplySort(queryParameters.SortBy); // Default sort is by Id (from GenericRepository or ApplySort extension)

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                                   .Take(queryParameters.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();
            return new PagedResult<TemplateUIElem>(items, queryParameters.PageNumber, queryParameters.PageSize, totalItems);
        }

        public async Task<TemplateUIElem?> GetTemplateUIElemByNameAsync(string name, string? includeProperties = null)
        {
            IQueryable<TemplateUIElem> query = _dbSet.Where(t => t.Name == name);
            query = query.IncludeProperties(includeProperties);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, long? currentId = null)
        {
            var query = _dbSet.Where(t => t.Name == name);
            if (currentId.HasValue)
            {
                query = query.Where(t => t.Id != currentId.Value);
            }
            return !await query.AnyAsync();
        }

        public Task<bool> IsElemTypeAndNameUniqueAsync(int elemType, string name, long? currentId = null)
        {
            // Implementation of IsElemTypeAndNameUniqueAsync method
            // throw new NotImplementedException();
            return Task.FromResult(false); // Placeholder to remove CS1998
        }

        // No need to override GetByIdAsync or GetAllAsync from GenericRepository 
        // if TemplateUIElem does not have IsRemoved or IsDisabled fields.
        // The base GenericRepository methods will work correctly.
    }
} 