using JaCore.Api.Helpers; // For ApiError and ErrorType

namespace JaCore.Api.Helpers.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ApiError? Error { get; }

    protected Result(bool isSuccess, ApiError? error)
    {
        if (isSuccess && error != null && error != ApiError.None)
            throw new InvalidOperationException("Successful result cannot have an error unless it's ApiError.None.");
        if (!isSuccess && (error == null || error == ApiError.None))
            throw new InvalidOperationException("Failed result must have a non-None error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(ApiError error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(ApiError error) => new(default, false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value, bool isSuccess, ApiError? error)
        : base(isSuccess, error)
    {
        Value = value;
    }
}