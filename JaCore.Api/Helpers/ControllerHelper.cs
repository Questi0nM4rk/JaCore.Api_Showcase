using JaCore.Api.Helpers.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using JaCore.Api.Helpers;

namespace JaCore.Api.Helpers
{
    public static class ControllerHelper
    {
        public static IActionResult ToActionResult<T>(
            this Result<T> result,
            ControllerBase controller,
            string? createdAtActionName = null,
            object? routeValues = null)
        {
            if (result.IsSuccess)
            {
                if (result.Value == null || (typeof(T) == typeof(bool) && !(bool)(object)result.Value))
                {
                    // For bool results, if false, it might imply not found or no content for a delete/update that didn't fail but also didn't act.
                    // However, a successful bool:false from a Delete might mean "found and deleted" which is 204.
                    // This needs careful handling based on service method conventions.
                    // For now, treating null or bool:false from a successful operation as NoContent is a common pattern for void/bool service methods.
                    if (controller.Request.Method == HttpMethod.Delete.Method || 
                        (controller.Request.Method == HttpMethod.Put.Method && typeof(T) == typeof(bool) && result.Value != null && !(bool)(object)result.Value) ||
                        (controller.Request.Method == HttpMethod.Patch.Method && typeof(T) == typeof(bool) && result.Value != null && !(bool)(object)result.Value) )
                    {
                         return controller.NoContent();
                    }
                    // If it's a GET that successfully returned null (e.g. GetById found nothing but didn't error)
                    // it should be a NotFound. But Result should use IsFailure for this with Error.NotFound.
                    // If Value is null for a successful GET here, it's ambiguous. Assuming it means success with no data to show.
                    // For POST/PUT that returns the object, if Value is null, that's unexpected for a success. Should be IsFailure.
                }

                if (!string.IsNullOrEmpty(createdAtActionName) && routeValues != null && controller.Request.Method == HttpMethod.Post.Method)
                {
                    return controller.CreatedAtAction(createdAtActionName, routeValues, result.Value);
                }
                
                if (result.Value is bool && (bool)(object)result.Value == true && 
                    (controller.Request.Method == HttpMethod.Delete.Method || controller.Request.Method == HttpMethod.Put.Method || controller.Request.Method == HttpMethod.Patch.Method))
                {
                    return controller.NoContent(); // Successful bool true for CUD operations typically means 204
                }

                if (result.Value == null && controller.Request.Method != HttpMethod.Get.Method) // Success but no value for non-GET operations is 204
                {
                     return controller.NoContent();
                }

                return controller.Ok(result.Value);
            }

            // Handle Failure
            if (result.Error == null) // Should not happen with current Result pattern
            {
                return controller.StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }

            return result.Error.Type switch
            {
                ErrorType.Validation => controller.BadRequest(result.Error),
                ErrorType.NotFound => controller.NotFound(result.Error),
                ErrorType.Conflict => controller.Conflict(result.Error),
                ErrorType.Unauthorized => controller.Unauthorized(result.Error),
                ErrorType.Forbidden => controller.Forbid(), // Forbid() doesn't take a value
                ErrorType.ProcessFailure or ErrorType.Unexpected => controller.StatusCode((int)HttpStatusCode.InternalServerError, result.Error),
                _ => controller.StatusCode((int)HttpStatusCode.InternalServerError, result.Error),
            };
        }
    }
} 