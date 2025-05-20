using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;
using System.Threading.Tasks;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface ISupplierService
    {
        Task<Result<PagedResult<SupplierDto>>> GetAllSuppliersAsync(QueryParametersDto queryParameters, string? includeProperties = null);
        Task<Result<SupplierDto>> GetSupplierByIdAsync(long id, string? includeProperties = null);
        Task<Result<SupplierDto>> GetSupplierByNameAsync(string name, string? includeProperties = null);
        Task<Result<SupplierDto>> CreateSupplierAsync(CreateSupplierDto createSupplierDto);
        Task<Result<SupplierDto>> UpdateSupplierAsync(long id, UpdateSupplierDto updateSupplierDto);
        Task<Result<SupplierDto>> PatchSupplierAsync(long id, PatchSupplierDto patchSupplierDto);
        Task<Result<bool>> DeleteSupplierAsync(long id); // Soft delete
    }
} 