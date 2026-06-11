namespace SistemaRestaurante.Domain.Errors;

public static class ResultTExtensions
{
    public static Result<T> ToResult<T>(this T value) => Result<T>.FromValue(value);
    public static Result<T> ToResult<T>(this Error error) => Result<T>.FromError(error);
    public static Result<T> CastResult<T>(this Result result) => Result<T>.FromErrors([.. result.Errors!]);
}