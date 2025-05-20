using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        Task<PagedResult<Supplier>> GetAllAsync(QueryParametersDto queryParameters, string? includeProperties = null);
        Task<Supplier?> GetSupplierByNameAsync(string name, string? includeProperties = null);
        Task<bool> IsNameUniqueAsync(string name, long? currentId = null);
        // Add other Supplier-specific methods here if needed
    }
} 