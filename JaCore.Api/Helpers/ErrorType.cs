namespace JaCore.Api.Helpers;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    ProcessFailure, // For general processing errors
    Unexpected,      // For unhandled exceptions or truly unexpected states
    DatabaseError
} 