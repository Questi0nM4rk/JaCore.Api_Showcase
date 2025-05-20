using JaCore.Api.DTOs.Common;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceEntity = JaCore.Api.Entities.Device.Device;

namespace JaCore.Api.Repositories.Abstractions.Device
{
    public interface IDeviceRepository : IGenericRepository<DeviceEntity>
    {
        Task<PagedResult<DeviceEntity>> GetAllAsync(QueryParametersDto queryParameters, long? locationID = null, string? includeProperties = null);
        Task<DeviceEntity?> GetDeviceByNameAsync(string name, string? includeProperties = null);
        Task<IEnumerable<DeviceEntity>> GetDevicesByLocationIDAsync(long locationID, string? includeProperties = null);
        Task<bool> IsNameUniqueAsync(string name, string? currentDeviceID = null);
    }
} 