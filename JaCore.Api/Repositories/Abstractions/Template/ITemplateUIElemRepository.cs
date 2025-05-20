using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Template;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;
using JaCore.Api.Repositories.Abstractions; // For IGenericRepository

namespace JaCore.Api.Repositories.Abstractions.Template
{
    public interface ITemplateUIElemRepository : IGenericRepository<TemplateUIElem>
    {
        Task<PagedResult<TemplateUIElem>> GetAllAsync(QueryParametersDto queryParameters, string? includeProperties = null);
        Task<TemplateUIElem?> GetTemplateUIElemByNameAsync(string name, string? includeProperties = null);
        Task<bool> IsNameUniqueAsync(string name, long? currentId = null);
        // Add any TemplateUIElem-specific methods here if needed
    }
} 