
namespace SistemaRestaurante.Domain.Errors;

public static class ValidationErrors
{
    public static Error NullField(string field) => new Error("VAL-DOM-0001", $"O campo \"{field}\" nao pode ser nulo", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error StringTooLong(string field, string value, int max) => new Error("VAL-DOM-0002", $"'{field}' deve ter no maximo {max} caracteres (recebido: {value.Length})", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error StringTooShort(string field, string value, int min) => new Error("VAL-DOM-0003", $"'{field}' deve ter no minimo {min} caracteres (recebido: {value.Length})", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error EmptyGuid(string field) => new Error("VAL-DOM-0004", $"O campo {field} nao pode ser vazio", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error InvalidURL(string field, string value) => new Error("VAL-DOM-0005", $"\"{value}\" nao e um {field} valida", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error InvalidFormat(string field, string value, string typeName) => new Error("VAL-DOM-0006", $"\"{value}\" nao esta em um formato ideal para {typeName} em {field}", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error InvalidDate(string field, DateTime value) => new Error("VAL-DOM-0007", $"O valor \"{value}\" em \"{field}\" nao e valida", ErrorTypes.Validation, new() { { "fieldName", field } });
    public static Error NumberTooLow(string field, int value, int min) => new Error("VAL-DOM-0008", $"'{field}' deve ser no minimo {min}, valor encontrado: {value}.", ErrorTypes.Validation);
    public static Error NumberTooHigh(string field, int value, int max) => new Error("VAL-DOM-0009", $"'{field}' deve ser no maximo {max}, valor encontrado: {value}.", ErrorTypes.Validation);
    public static Error NumberTooLow(string field, double value, double min) => new Error("VAL-DOM-0008", $"'{field}' deve ser no minimo {min}, valor encontrado: {value}.", ErrorTypes.Validation);
    public static Error NumberTooHigh(string field, double value, double max) => new Error("VAL-DOM-0009", $"'{field}' deve ser no maximo {max}, valor encontrado: {value}.", ErrorTypes.Validation);
    public static Error InvalidEnum<E>(string field, E value) where E : Enum => new Error("VAL-DOM-0010", $"{field} não possui valor válido para o enum {nameof(E)}, valor encontrado: {value}", ErrorTypes.Validation);
    public static Dictionary<string, object> GetFieldErros(IReadOnlyList<Error>? errors)
    {
        Dictionary<string, object> invalidFields = [];

        foreach (Error error in errors ?? [])
        {
            var metadata = error.Metadata ?? [];

            if (metadata.Values.Count == 0) continue;
            if (metadata.TryGetValue("fieldName", out object? fieldName) == false) continue;

            invalidFields.Add((string)fieldName, error.Message);
        }

        return invalidFields;
    }
}