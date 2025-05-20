namespace JaCore.Api.Helpers;

/// <summary>
/// Represents a standardized API error.
/// </summary>
/// <param name="Type">The type of the error.</param>
/// <param name="Code">A unique code for the error.</param>
/// <param name="Message">A descriptive message for the error.</param>
public sealed record ApiError(ErrorType Type, string Code, string Message)
{
    /// <summary>
    /// Represents a non-error or no specific error.
    /// </summary>
    public static readonly ApiError None = new(default!, string.Empty, string.Empty);
} 