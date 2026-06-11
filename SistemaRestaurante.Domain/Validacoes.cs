using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain;

public class Validacoes
{
    public static Result ValidarTexto(string nomeCampo, string valor, int min = 0, int max = int.MaxValue, bool nula = false)
    {
        List<Error> errors = [];

        if (valor is null)
        {
            if (nula == false)
            {
                errors.Add(ValidationErrors.NullField(nomeCampo));
                return errors;
            }

            return Result.Success();
        }

        if (valor.Length > max)
        {
            errors.Add(ValidationErrors.StringTooLong(nomeCampo, valor, max));
        }

        if (valor.Length < min)
        {
            errors.Add(ValidationErrors.StringTooShort(nomeCampo, valor, max));
        }

        return errors;
    }

    public static Result ValidarNumero(string nomeCampo, int valor, int min, int max)
    {
        List<Error> errors = [];

        if (valor > max)
        {
            errors.Add(ValidationErrors.NumberTooHigh(nomeCampo, valor, max));
        }

        if (valor < min)
        {
            errors.Add(ValidationErrors.NumberTooLow(nomeCampo, valor, max));
        }

        return errors;
    }

    public static Result ValidarNumero(string nomeCampo, double valor, double min, double max)
    {
        List<Error> errors = [];

        if (valor > max)
        {
            errors.Add(ValidationErrors.NumberTooHigh(nomeCampo, valor, max));
        }

        if (valor < min)
        {
            errors.Add(ValidationErrors.NumberTooLow(nomeCampo, valor, max));
        }

        return errors;
    }
}