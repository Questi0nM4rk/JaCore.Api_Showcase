using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;
using Microsoft.AspNetCore.Http; // Added for StatusCodes
// Alias for the error record type used in responses
using ResponseError = JaCore.Api.Helpers.ApiError;

namespace JaCore.Api.Controllers.Device
{
    // [Route(ApiConstants.BasePaths.Devices)] // Base route for Devices - Commenting out or removing if redundant
    [ApiController]
    [ApiVersion("1.0")]
    [Route(ApiConstants.BasePaths.Device)] // Use constant for base path
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DevicesController> _logger;
        private readonly IObjectModelValidator _objectModelValidator;

        public DevicesController(
            IDeviceService deviceService, 
            ILogger<DevicesController> logger,
            IObjectModelValidator objectModelValidator) // For PATCH validation
        {
            _deviceService = deviceService;
            _logger = logger;
            _objectModelValidator = objectModelValidator;
        }

        [HttpGet] // Root for this controller, e.g., api/v1/device which is ApiConstants.DeviceRoutes.GetAll
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetAllDevices([FromQuery] QueryParametersDto queryParams, [FromQuery] long? locationId)
        {
            var result = await _deviceService.GetAllDevicesAsync(queryParams, locationId);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.DeviceId)] // Path is relative to controller's base: {deviceId:long}
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetDeviceById(long deviceId, [FromQuery] string? includeProperties = null)
        {
            // Example: includeProperties="Location,DeviceCard,DeviceCard.Supplier,DeviceCard.Service"
            var result = await _deviceService.GetDeviceByIdAsync(deviceId, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.DeviceEndpoints.ByName, Name = nameof(GetDeviceByName))] // Path: name/{name}
        [Authorize(Policy = ApiConstants.Policies.UserAccess)] // Assuming UserAccess is a defined policy
        [ProducesResponseType(typeof(DeviceDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> GetDeviceByName(string name)
        {
            var result = await _deviceService.GetDeviceByNameAsync(name);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.DeviceEndpoints.ByLocation)] // Path: by-location/{locationId:long}
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetDevicesByLocation(long locationId, [FromQuery] QueryParametersDto queryParams)
        {
            // The GetAllDevicesAsync can filter by locationId.
            var result = await _deviceService.GetAllDevicesAsync(queryParams, locationId);
            return result.ToActionResult(this);
        }

        [HttpPost] // POST to ApiConstants.BasePaths.Device which is ApiConstants.DeviceRoutes.Create
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceDto createDto)
        {
            var result = await _deviceService.CreateDeviceAsync(createDto);
            if (!result.IsSuccess) return result.ToActionResult(this);
            // Ensure the route value 'deviceId' matches the parameter name in GetDeviceById
            return CreatedAtAction(nameof(GetDeviceById), new { deviceId = result.Value!.ID }, result.Value);
        }

        [HttpPut(ApiConstants.IdParams.DeviceId)] // Path: {deviceId:long}
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateDevice(long deviceId, [FromBody] UpdateDeviceDto updateDto)
        {
            var result = await _deviceService.UpdateDeviceAsync(deviceId, updateDto);
            return result.ToActionResult(this);
        }

        [HttpPatch(ApiConstants.IdParams.DeviceId)] // Path: {deviceId:long}
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> PatchDevice(long deviceId, [FromBody] PatchDeviceDto patchDto)
        {
            var result = await _deviceService.PatchDeviceAsync(deviceId, patchDto);
            return result.ToActionResult(this);
        }

        [HttpPatch(ApiConstants.IdParams.DeviceId + "/" + ApiConstants.DeviceEndpoints.UpdateStatus)] // Path: {deviceId:long}/status
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateDeviceStatus(long deviceId, [FromBody] DeviceStatusPatchDto statusDto)
        {
            Result<bool> result;
            if (statusDto.IsDisabled)
            {
                result = await _deviceService.DisableDeviceAsync(deviceId);
            }
            else
            {
                result = await _deviceService.EnableDeviceAsync(deviceId);
            }
            // To return DeviceDto, we might need to fetch it after status update if Disable/EnableDeviceAsync return bool.
            // For now, returning based on the bool result.
            if (!result.IsSuccess) return result.ToActionResult(this);
            if (result.Value) return NoContent(); // Or Ok(statusDto) or fetch the updated device
            return StatusCode(StatusCodes.Status500InternalServerError, JaCore.Api.Helpers.ErrorHelper.Unexpected("Failed to update device status."));
        }

        [HttpDelete(ApiConstants.IdParams.DeviceId)] // Path: {deviceId:long}
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        public async Task<IActionResult> DeleteDevice(long deviceId)
        {
            var result = await _deviceService.DeleteDeviceAsync(deviceId);
            return result.ToActionResult(this);
        }

        [HttpPut(ApiConstants.IdParams.DeviceId + "/" + ApiConstants.DeviceEndpoints.LinkLocation)] // Path: {deviceId:long}/location
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> LinkDeviceToLocation(long deviceId, [FromBody] LinkLocationToDeviceDto linkDto)
        {
            var result = await _deviceService.LinkDeviceToLocationAsync(deviceId, linkDto);
            return result.ToActionResult(this);
        }

        [HttpPost(ApiConstants.IdParams.DeviceId + "/" + ApiConstants.DeviceEndpoints.Disable, Name = nameof(DisableDevice))] // Path: {deviceId:long}/disable
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)] // Corrected from ManagementAccess if ManageAccess is the standard
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> DisableDevice(long deviceId)
        {
            var result = await _deviceService.DisableDeviceAsync(deviceId);
            return result.ToActionResult(this);
        }

        [HttpPost(ApiConstants.IdParams.DeviceId + "/" + ApiConstants.DeviceEndpoints.Enable, Name = nameof(EnableDevice))] // Path: {deviceId:long}/enable
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)] // Corrected from ManagementAccess
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> EnableDevice(long deviceId)
        {
            var result = await _deviceService.EnableDeviceAsync(deviceId);
            return result.ToActionResult(this);
        }
    }
}

 