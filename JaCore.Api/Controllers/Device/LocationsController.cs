using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
// Alias for the error record type used in responses
using ResponseError = JaCore.Api.Helpers.ApiError;

namespace JaCore.Api.Controllers.Device
{
    // Route will be api/v1/location
    [Route(ApiConstants.BasePaths.Location)]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize] // Default authorization for the controller
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService locationService, ILogger<LocationsController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        [ProducesResponseType(typeof(PagedResult<LocationDto>), 200)]
        public async Task<IActionResult> GetAllLocations([FromQuery] QueryParametersDto queryParameters)
        {
            var result = await _locationService.GetAllLocationsAsync(queryParameters);
            if (result.IsSuccess && result.Value != null)
            {
                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new 
                {
                    result.Value.TotalCount,
                    result.Value.PageSize,
                    result.Value.PageNumber,
                    result.Value.TotalPages,
                    result.Value.HasNextPage,
                    result.Value.HasPreviousPage
                }));
                return Ok(result.Value.Items);
            }
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.IdParams.LocationId)]
        [Authorize(Policy = ApiConstants.Policies.AuthenticatedUser)]
        [ProducesResponseType(typeof(LocationDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> GetLocationById(long locationId, [FromQuery] string? includeProperties = null)
        {
            var result = await _locationService.GetLocationByIdAsync(locationId, includeProperties);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.LocationEndpoints.ByName, Name = nameof(GetLocationByName))]
        [Authorize(Policy = ApiConstants.Policies.UserAccess)]
        [ProducesResponseType(typeof(LocationDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> GetLocationByName(string name, [FromQuery] string? include)
        {
            var result = await _locationService.GetLocationByNameAsync(name, include);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        [ProducesResponseType(typeof(LocationDto), 201)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDto createLocationDto)
        {
            var result = await _locationService.CreateLocationAsync(createLocationDto);
            return result.ToActionResult(this, nameof(GetLocationById), new { locationId = result.IsSuccess ? result.Value?.ID : 0 });
        }

        [HttpPut(ApiConstants.IdParams.LocationId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> UpdateLocation(long locationId, [FromBody] UpdateLocationDto updateLocationDto)
        {
            var result = await _locationService.UpdateLocationAsync(locationId, updateLocationDto);
            return result.ToActionResult(this);
        }

        [HttpDelete(ApiConstants.IdParams.LocationId)]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> DeleteLocation(long locationId)
        {
            var result = await _locationService.DeleteLocationAsync(locationId);
            return result.ToActionResult(this);
        }

        // PATCH api/v1.0/location/{locationId:long}
        [HttpPatch(ApiConstants.IdParams.LocationId)]
        [Authorize(Policy = ApiConstants.Policies.ManageAccess)] // Or AdminAccess depending on requirements
        [ProducesResponseType(typeof(LocationDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> PatchLocation(long locationId, [FromBody] PatchLocationDto patchDto)
        {
            if (patchDto == null)
            {
                return BadRequest(JaCore.Api.Helpers.ErrorHelper.Validation("Patch document cannot be null."));
            }
            var result = await _locationService.PatchLocationAsync(locationId, patchDto);
            return result.ToActionResult(this);
        }
    }
} 