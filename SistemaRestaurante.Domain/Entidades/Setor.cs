using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Setor
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    private const int MIN_CARACTERES_NOME = 5;
    private const int MAX_CARACTERES_NOME = 255;
    public TipoSetor Tipo { get; private set; }

    private Setor(int id, string nome, TipoSetor tipo)
    {
        Id = id;
        Nome = nome;
        Tipo = tipo;
    }

    public static Result<Setor> Criar(int id, string nome, TipoSetor tipo)
    {
        var result = Result.All(
            Validacoes.ValidarNumero(nameof(id), id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(nome), nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME)
        );

        if (result.IsError)
        {
            return (Result<Setor>)result;
        }

        return new Setor(id, nome, tipo);
    }

    public Result Atualizar(string nome)
    {
        var result = Validacoes.ValidarTexto(nameof(nome), nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME);

        if (result.IsError)
        {
            return result;
        }

        Nome = nome;
        return Result.Success();
    }

    public Result Validar()
    {
        var result = Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(Nome), Nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME)
        );

        return result;
    }
}
