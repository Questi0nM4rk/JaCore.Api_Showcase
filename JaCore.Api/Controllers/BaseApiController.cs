using JaCore.Api.Helpers;
using JaCore.Api.Helpers.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace JaCore.Api.Controllers
{
    [ApiController]
    [ApiVersion(ApiConstants.Versions.VersionString)] // Use the string version for the attribute
    // [Authorize] // Apply a default authorization policy if all controllers require it. Specific policies can override.
    // Consider [Authorize(Policy = ApiConstants.Policies.AuthBased)] as a default if most endpoints require authenticated users.
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseApiController(ILogger logger)
        {
            _logger = logger;
        }

        protected IActionResult HandleResult<T>(Result<T> result, string? createdAtActionName = null, object? routeValues = null)
        {
            if (result.IsSuccess)
            {
                if (result.Value == null && createdAtActionName == null) // For non-creation GETs that might return null for not found by specific criteria (e.g. GetByName)
                {
                    // This scenario should ideally be handled by the service returning Result.Failure with NotFound.
                    // If a service *can* return a successful result with a null value for a GET, 
                    // and it's not a 'Not Found' scenario, then Ok(null) might be acceptable.
                    // However, for typical GetById/GetByName, null usually means NotFound.
                    // Let's assume services correctly return Failure for NotFound.
                    // If it's a successful non-creation GET and value is null, it implies an empty successful result (e.g. empty list handled by Ok(Value) directly)
                    // For a single entity GET, if value is null and IsSuccess is true, this is ambiguous from service layer.
                    // For now, we assume successful non-null Value or successful null from Create (which should be CreatedAtAction).
                }

                if (createdAtActionName != null)
                {
                    return CreatedAtAction(createdAtActionName, routeValues, result.Value);
                }
                return Ok(result.Value);
            }

            return HandleFailure(result.Error!);
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent(); // Typically for Update/Delete operations
            }
            return HandleFailure(result.Error!);
        }

        private IActionResult HandleFailure(ApiError error)
        {
            // You can add more sophisticated error mapping here based on error.Code if needed
            // For example, map specific domain error codes to more specific HTTP status codes.
            // The properties Code and Type are available on the Error record.
            return error.Type switch
            {
                ErrorType.NotFound => NotFound(error),
                ErrorType.Validation => BadRequest(error),
                ErrorType.Conflict => Conflict(error),
                ErrorType.Unauthorized => Unauthorized(error),
                ErrorType.Forbidden => Forbid(),
                // Defaulting to BadRequest for Failure, ProcessFailure, Custom
                _ => BadRequest(error)
            };
        }
    }
} 