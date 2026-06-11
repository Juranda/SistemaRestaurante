using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Mesa
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    private const int MIN_CARACTERES_NOME = 5;
    private const int MAX_CARACTERES_NOME = 50;

    private Mesa(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public static Result<Mesa> Criar(int id, string nome)
    {
        var mesa = new Mesa(id, nome);
        var result = mesa.Validar();

        if (result.IsError)
        {
            return (Result<Mesa>)result;
        }

        return mesa;
    }

    internal Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(Nome), Nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME)
        );
    }
}
