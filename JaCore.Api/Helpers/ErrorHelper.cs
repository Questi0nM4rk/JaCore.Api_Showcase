namespace JaCore.Api.Helpers;

public static class ErrorHelper
{
    // Standard error instances or factory methods
    public static ApiError NotFound(string entityName, string? identifier = null)
    {
        string message = identifier == null
            ? $"{entityName} not found."
            : $"{entityName} with identifier '{identifier}' not found.";
        return new ApiError(ErrorType.NotFound, $"{entityName.ToUpperInvariant()}_NOT_FOUND", message);
    }

    public static ApiError Validation(string message, string? fieldName = null)
    {
        string code = fieldName == null ? "VALIDATION_ERROR" : $"VALIDATION_ERROR_{fieldName.ToUpperInvariant()}";
        return new ApiError(ErrorType.Validation, code, message);
    }

    public static ApiError Conflict(string message)
    {
        return new ApiError(ErrorType.Conflict, "CONFLICT", message);
    }
    
    public static ApiError Unauthorized(string message = "Unauthorized access.")
    {
        return new ApiError(ErrorType.Unauthorized, "UNAUTHORIZED", message);
    }

    public static ApiError Forbidden(string message = "Forbidden access.")
    {
        return new ApiError(ErrorType.Forbidden, "FORBIDDEN", message);
    }

    public static ApiError ProcessFailure(string message, string? code = null)
    {
        return new ApiError(ErrorType.ProcessFailure, code ?? "PROCESS_FAILURE", message);
    }

    public static ApiError Unexpected(string message = "An unexpected error occurred.")
    {
        return new ApiError(ErrorType.Unexpected, "UNEXPECTED_ERROR", message);
    }

    public static ApiError DatabaseError(string message = "A database error occurred.")
    {
        return new ApiError(ErrorType.DatabaseError, "DATABASE_ERROR", message);
    }
} 