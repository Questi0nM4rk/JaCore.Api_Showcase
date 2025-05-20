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
    [Route(ApiConstants.BasePaths.Service)]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceEntityService _serviceEntityService;

        public ServicesController(IServiceEntityService serviceEntityService)
        {
            _serviceEntityService = serviceEntityService;
        }

        [HttpGet]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetAllServices([FromQuery] QueryParametersDto queryParameters, [FromQuery] string? includeProperties = null)
        {
            var result = await _serviceEntityService.GetAllServicesAsync(queryParameters, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.ServiceId)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetServiceById(long serviceId, [FromQuery] string? includeProperties = null)
        {
            var result = await _serviceEntityService.GetServiceByIdAsync(serviceId, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto createDto)
        {
            var result = await _serviceEntityService.CreateServiceAsync(createDto);
            return result.ToActionResult(this, nameof(GetServiceById), new { serviceId = result.IsSuccess ? result.Value?.ID : 0 });
        }

        [HttpPut(ApiConstants.IdParams.ServiceId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateService(long serviceId, [FromBody] UpdateServiceDto updateDto)
        {
            var result = await _serviceEntityService.UpdateServiceAsync(serviceId, updateDto);
            return result.ToActionResult(this);
        }

        [HttpPatch(ApiConstants.IdParams.ServiceId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> PatchService(long serviceId, [FromBody] PatchServiceDto patchDto)
        {
            var result = await _serviceEntityService.PatchServiceAsync(serviceId, patchDto);
            return result.ToActionResult(this);
        }

        [HttpDelete(ApiConstants.IdParams.ServiceId)]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        public async Task<IActionResult> DeleteService(long serviceId)
        {
            var result = await _serviceEntityService.DeleteServiceAsync(serviceId);
            return result.ToActionResult(this);
        }
    }
} 