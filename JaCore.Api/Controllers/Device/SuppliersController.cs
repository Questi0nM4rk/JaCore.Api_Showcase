using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route(ApiConstants.BasePaths.Supplier)]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetAllSuppliers([FromQuery] QueryParametersDto queryParameters, [FromQuery] string? includeProperties = null)
        {
            var result = await _supplierService.GetAllSuppliersAsync(queryParameters, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.SupplierId)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetSupplierById(long supplierId, [FromQuery] string? includeProperties = null)
        {
            var result = await _supplierService.GetSupplierByIdAsync(supplierId, includeProperties);
            return result.ToActionResult(this);
        }
        
        [HttpGet(ApiConstants.SupplierEndpoints.ByName)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetSupplierByName(string name, [FromQuery] string? includeProperties = null)
        {
            var result = await _supplierService.GetSupplierByNameAsync(name, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto createDto)
        {
            var result = await _supplierService.CreateSupplierAsync(createDto);
            return result.ToActionResult(this, nameof(GetSupplierById), new { supplierId = result.IsSuccess ? result.Value?.ID : 0 });
        }

        [HttpPut(ApiConstants.IdParams.SupplierId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateSupplier(long supplierId, [FromBody] UpdateSupplierDto updateDto)
        {
            var result = await _supplierService.UpdateSupplierAsync(supplierId, updateDto);
            return result.ToActionResult(this);
        }

        [HttpPatch(ApiConstants.IdParams.SupplierId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> PatchSupplier(long supplierId, [FromBody] PatchSupplierDto patchDto)
        {
            var result = await _supplierService.PatchSupplierAsync(supplierId, patchDto);
            return result.ToActionResult(this);
        }

        [HttpDelete(ApiConstants.IdParams.SupplierId)]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        public async Task<IActionResult> DeleteSupplier(long supplierId)
        {
            var result = await _supplierService.DeleteSupplierAsync(supplierId);
            return result.ToActionResult(this);
        }
    }
} 