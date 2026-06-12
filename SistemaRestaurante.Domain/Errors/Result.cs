using System.Text.Json;

namespace SistemaRestaurante.Domain.Errors;

public class Result
{
    public bool IsSuccess => _Errors is null || _Errors.Count == 0;
    public bool IsError => !IsSuccess;
    protected List<Error>? _Errors { get; private set; }
    public IReadOnlyList<Error>? Errors => _Errors;
    public ErrorTypes? ErrorType => _Errors?[0].ErrorType;

    protected Result(Error? error)
    {
        if (error is not null)
            _Errors = [error];
    }

    protected Result(List<Error>? errors)
    {
        if (errors is not null)
            _Errors = errors;
    }
    public static Result FromError(Error error) => new(error);
    public static Result FromErrors(List<Error> errors) => new(errors);
    public static Result Success() => new((Error?)null);

    public Result CombineResult(Result other)
    {
        _Errors ??= [];
        other._Errors ??= [];

        _Errors.AddRange(other._Errors);

        if (_Errors.Count == 0)
        {
            _Errors = null;
            other._Errors = null;
            return this;
        }

        return this;
    }

    public Result AddError(Error error)
    {
        _Errors ??= [];
        _Errors.Add(error);
        return this;
    }

    public static Result All(params Result[] results)
    {
        int avarageErrorsLenght = results.Length * 2;
        List<Error> errors = new(avarageErrorsLenght);

        foreach (var result in results)
        {
            if (result.IsSuccess) continue;

            errors.AddRange(result.Errors!);
        }

        return FromErrors(errors);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public static implicit operator Result(Error error) => FromError(error);
    public static implicit operator Result(List<Error> errors) => FromErrors(errors);
}


public sealed class Result<T> : Result
{
    public T? Value { get; private set; }

    private Result(T? value, Error? error) : base(error)
    {
        Value = value;
    }
    private Result(T? value, List<Error>? errors) : base(errors)
    {
        Value = value;
    }

    public void Match(Action<T> OnValue, Action<List<Error>> OnError)
    {
        if (_Errors is not null)
        {
            OnError(_Errors);
            return;
        }

        if (Value is not null)
        {
            OnValue(Value);
            return;
        }

        throw new Exception("Invalid state");
    }
    new public static Result<T> FromError(Error error) => new(default, error);
    new public static Result<T> FromErrors(List<Error> errors) => new(default, errors);
    public static Result<T> FromValue(T value) => new(value, (Error?)null);
    public static Result<T> FromResult(Result result) => FromErrors((result.Errors ?? []).ToList());
    public static Result<T> Sucess(T value) => new(value, (Error?)null);

    public static implicit operator Result<T>(T value) => FromValue(value);
    public static implicit operator Result<T>(Error error) => FromError(error);
    public static implicit operator Result<T>(List<Error> errors) => FromErrors(errors);
    public static implicit operator string(Result<T> result) => result.ToString();
    public override string ToString()
    {
        if (IsSuccess)
        {
            return $"Result = Value {Value}";
        }

        return base.ToString();
    }
}