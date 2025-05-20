using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;
using System.Collections.Generic; // For List in UpdateOrder

namespace JaCore.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route(ApiConstants.BasePaths.DeviceOperationCollection)]
    [Authorize]
    public class DeviceOperationsController : ControllerBase
    {
        private readonly IDeviceOperationService _opService;

        public DeviceOperationsController(IDeviceOperationService opService)
        {
            _opService = opService;
        }

        [HttpGet(ApiConstants.DeviceOperationEndpoints.GetAll)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetOperationsForDeviceCard(long deviceId, long cardId, [FromQuery] QueryParametersDto queryParams)
        {
            var result = await _opService.GetDeviceOperationsByDeviceCardIdAsync(cardId, queryParams);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.OperationId)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetSingleOperationForDeviceCard(long deviceId, long cardId, long opId)
        {
            var result = await _opService.GetDeviceOperationByIdAndDeviceCardIdAsync(opId, cardId);
            return result.ToActionResult(this);
        }

        [HttpPost(ApiConstants.DeviceOperationEndpoints.Create)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> CreateOperation(long deviceId, long cardId, [FromBody] CreateDeviceOperationDto createDto)
        {
            var result = await _opService.CreateDeviceOperationAsync(cardId, createDto);
            if (!result.IsSuccess) return result.ToActionResult(this);
            return CreatedAtAction(nameof(GetSingleOperationForDeviceCard), new { deviceId = deviceId, cardId = cardId, opId = result.Value!.Id }, result.Value);
        }

        [HttpPut(ApiConstants.IdParams.OperationId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateOperation(long deviceId, long cardId, long opId, [FromBody] UpdateDeviceOperationDto updateDto)
        {
            var result = await _opService.UpdateDeviceOperationAsync(cardId, opId, updateDto);
            return result.ToActionResult(this);
        }

        [HttpPatch(ApiConstants.DeviceOperationEndpoints.Reorder)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateOperationsOrder(long deviceId, long cardId, [FromBody] UpdateDeviceOperationsOrderDto orderedOperationsWrapperDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _opService.UpdateDeviceOperationsOrderAsync(cardId, orderedOperationsWrapperDto.Operations);
            
            if (!result.IsSuccess)
            {
                if (result.Error != null)
                {
                    switch (result.Error.Type)
                    {
                        case ErrorType.NotFound:
                            return NotFound(result.Error);
                        case ErrorType.Validation:
                        case ErrorType.Conflict:
                            return BadRequest(result.Error);
                        default:
                            return StatusCode(500, result.Error);
                    }
                }
                return StatusCode(500, "An unexpected error occurred while updating operation order.");
            }
            return Ok(result.Value);
        }
        
        [HttpPatch(ApiConstants.IdParams.OperationId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> PatchOperation(long deviceId, long cardId, long opId, [FromBody] PatchDeviceOperationDto patchDto)
        {
            var result = await _opService.PatchDeviceOperationAsync(cardId, opId, patchDto);
            return result.ToActionResult(this);
        }

        [HttpDelete(ApiConstants.IdParams.OperationId)]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        public async Task<IActionResult> DeleteOperation(long deviceId, long cardId, long opId)
        {
            var result = await _opService.DeleteDeviceOperationAsync(opId, cardId);
            return result.ToActionResult(this);
        }
    }
} 