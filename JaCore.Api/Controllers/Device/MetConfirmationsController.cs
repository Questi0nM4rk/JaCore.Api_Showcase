using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results; // Added for Result<T>

namespace JaCore.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/metconfirmation")]
    [Authorize]
    public class MetConfirmationsController : ControllerBase
    {
        private readonly IMetConfirmationService _metConfirmationService;

        public MetConfirmationsController(IMetConfirmationService metConfirmationService)
        {
            _metConfirmationService = metConfirmationService;
        }

        [HttpGet]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetAllMetConfirmations([FromQuery] QueryParametersDto queryParameters)
        {
            var result = await _metConfirmationService.GetAllMetConfirmationsAsync(queryParameters);
            return result.ToActionResult(this);
        }

        [HttpGet("{id:long}")]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        public async Task<IActionResult> GetMetConfirmationById(long id, [FromQuery] string? includeProperties = null)
        {
            var result = await _metConfirmationService.GetMetConfirmationByIdAsync(id, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> CreateMetConfirmation([FromBody] CreateMetConfirmationDto createDto)
        {
            var result = await _metConfirmationService.CreateMetConfirmationAsync(createDto);
            return result.ToActionResult(this, nameof(GetMetConfirmationById), new { id = result.IsSuccess ? result.Value?.ID : 0 });
        }

        [HttpPut("{id:long}")]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> UpdateMetConfirmation(long id, [FromBody] UpdateMetConfirmationDto updateDto)
        {
            var result = await _metConfirmationService.UpdateMetConfirmationAsync(id, updateDto);
            return result.ToActionResult(this);
        }

        [HttpPatch("{id:long}")]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        public async Task<IActionResult> PatchMetConfirmation(long id, [FromBody] PatchMetConfirmationDto patchDto)
        {
            var result = await _metConfirmationService.PatchMetConfirmationAsync(id, patchDto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:long}")]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        public async Task<IActionResult> DeleteMetConfirmation(long id)
        {
            var result = await _metConfirmationService.DeleteMetConfirmationAsync(id);
            return result.ToActionResult(this);
        }
    }
} 