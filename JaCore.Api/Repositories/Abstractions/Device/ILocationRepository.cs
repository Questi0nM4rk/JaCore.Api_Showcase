using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using System.Collections.Generic;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<Location?> GetLocationByNameAsync(string name, string? includeProperties = null);
        Task<PagedResult<Location>> GetAllAsync(QueryParametersDto queryParameters);
        Task<bool> IsNameUniqueAsync(string name, long? currentId = null);
        // Add other Location-specific methods here if needed
    }
} 