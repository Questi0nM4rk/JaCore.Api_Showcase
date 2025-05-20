using System.Linq.Expressions;
using JaCore.Api.DTOs.Common; // For QueryParametersDto
using JaCore.Api.Helpers.Results; // For PagedResult

namespace JaCore.Api.Repositories.Abstractions
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetByIdAsync(object id, string? includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity); // Typically synchronous as it just marks state
        void Remove(T entity); // Typically synchronous
        void RemoveRange(IEnumerable<T> entities); // Typically synchronous
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task RemoveAsync(object id); // For soft delete by Id
    }
} 