namespace AudioBooks.Domain.Abstractions;

public class Result<T>
{
    private Result(T? value, Error? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public static Result<T> Success(T value) => new Result<T>(value, null, true);
    public static Result<T> Failure(Error error) => new Result<T>(default, error, false);
}
