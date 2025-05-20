using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    // Interface for ServiceEntity repository
    public interface IServiceEntityRepository : IGenericRepository<ServiceEntity>
    {
        Task<PagedResult<ServiceEntity>> GetAllAsync(QueryParametersDto queryParameters);
        // Add any ServiceEntity-specific methods here if needed
    }
} 