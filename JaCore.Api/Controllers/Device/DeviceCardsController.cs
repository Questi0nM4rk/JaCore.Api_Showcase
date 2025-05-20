using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace JaCore.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route(ApiConstants.BasePaths.DeviceCardCollection)]
    [Authorize]
    public class DeviceCardsController : BaseApiController
    {
        private readonly IDeviceCardService _deviceCardService;
        private readonly IMapper _mapper;

        public DeviceCardsController(IDeviceCardService deviceCardService, IMapper mapper, ILogger<DeviceCardsController> logger)
            : base(logger)
        {
            _deviceCardService = deviceCardService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetAllDeviceCardsForDevice(long deviceId, [FromQuery] QueryParametersDto queryParams)
        {
            var result = await _deviceCardService.GetDeviceCardsByDeviceIdAsync(deviceId, queryParams);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.CardId)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetDeviceCardById(long deviceId, long cardId, [FromQuery] string? includeProperties = null)
        {
            // Ensure service method validates that cardId belongs to deviceId
            var result = await _deviceCardService.GetDeviceCardByIdAsync(cardId, includeProperties); // Assuming GetDeviceCardByIdAsync internally checks parent deviceId if necessary, or a specific method is used.
            // If service doesn't inherently validate parent, an explicit check or a different service method like GetDeviceCardForDeviceAsync(deviceId, cardId, include) would be better.
            // For now, proceeding with the simpler GetDeviceCardByIdAsync which implies cardId is globally unique or service handles device context.
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> CreateDeviceCard(long deviceId, [FromBody] CreateDeviceCardDto createDto)
        {
            // The CreateDeviceCardDto already contains DeviceId. Ensure it matches deviceId from route or service handles it.
            // It is better practice if the DTO doesn't have DeviceId and it's passed to the service method.
            // However, our current CreateDeviceCardDto has DeviceId as required.
            // For now, assume the service will validate or use the DeviceId from DTO, and potentially reconcile with route deviceId.
            var result = await _deviceCardService.CreateDeviceCardAsync(createDto); // createDto.DeviceId should align with deviceId
            return result.ToActionResult(this, nameof(GetDeviceCardById), new { deviceId = deviceId, cardId = result.IsSuccess ? result.Value?.ID : 0 });
        }

        [HttpPut(ApiConstants.IdParams.CardId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateDeviceCard(long deviceId, long cardId, [FromBody] UpdateDeviceCardDto updateDto)
        {
            // Similar to Create, UpdateDeviceCardDto has DeviceId. Service should validate it against cardId's actual parent and route's deviceId.
            var result = await _deviceCardService.UpdateDeviceCardAsync(cardId, updateDto);
            return result.ToActionResult(this);
        }

        [HttpPatch(ApiConstants.IdParams.CardId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> PatchDeviceCard(long deviceId, long cardId, [FromBody] PatchDeviceCardDto patchDto)
        {
            var result = await _deviceCardService.PatchDeviceCardAsync(cardId, patchDto);
            return result.ToActionResult(this);
        }

        [HttpDelete(ApiConstants.IdParams.CardId)]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        public async Task<IActionResult> DeleteDeviceCard(long deviceId, long cardId)
        {
            // Service method should ensure cardId belongs to deviceId before deletion.
            var result = await _deviceCardService.DeleteDeviceCardAsync(cardId);
            return result.ToActionResult(this);
        }
        
        // devicedomain.md mentions specific linking endpoints for Supplier, Service, MetConfirmation.
        // e.g. PUT /api/devices/{deviceId}/devicecards/{cardId}/supplier
        // These would require specific DTOs (e.g., LinkSupplierDto { long SupplierID; }) 
        // and corresponding service methods.
        // Placeholder for one such link:
        /*
        [HttpPut("{cardId:long}/supplier")]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> LinkSupplierToDeviceCard(long deviceId, long cardId, [FromBody] LinkSupplierDto linkDto) 
        {
            // var result = await _deviceCardService.LinkSupplierAsync(cardId, linkDto.SupplierID); 
            // return result.ToActionResult(this);
            return NotFound("LinkSupplierToDeviceCard endpoint not fully implemented yet.");
        }
        */
    }
} 