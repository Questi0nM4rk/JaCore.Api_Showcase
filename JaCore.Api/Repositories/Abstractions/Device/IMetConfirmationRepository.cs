using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface IMetConfirmationRepository : IGenericRepository<MetConfirmation>
    {
        Task<PagedResult<MetConfirmation>> GetAllAsync(QueryParametersDto queryParameters);
        Task<bool> IsNameUniqueAsync(string name, long? currentId = null);
        // Add other MetConfirmation-specific methods here if needed
    }
} 