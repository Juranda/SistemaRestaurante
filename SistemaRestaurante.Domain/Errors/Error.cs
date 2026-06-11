using System.Text.Json;

namespace SistemaRestaurante.Domain.Errors;

public record Error(string Code, string Message, ErrorTypes ErrorType, Dictionary<string, object>? Metadata = null)
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, inputType: GetType());
    }

    public static implicit operator string(Error error)
    {
        return error.ToString();
    }
}
