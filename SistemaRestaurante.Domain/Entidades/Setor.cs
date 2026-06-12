using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Setor
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public const int MIN_CARACTERES_NOME = 5;
    public const int MAX_CARACTERES_NOME = 255;
    public TipoSetor Tipo { get; private set; }

    private Setor(int id, string nome, TipoSetor tipo)
    {
        Id = id;
        Nome = nome;
        Tipo = tipo;
    }

    public static Result<Setor> Criar(int id, string nome, TipoSetor tipo)
    {
        var setor = new Setor(id, nome, tipo);
        var result = setor.Validar();

        if (result.IsError)
        {
            return (Result<Setor>)result;
        }

        return setor;
    }

    public Result Atualizar(string nome, TipoSetor tipo)
    {
        var setor = new Setor(Id, nome, tipo);
        var result = setor.Validar();

        if (result.IsError)
        {
            return (Result<Setor>)result;
        }

        Nome = nome;
        Tipo = tipo;
        return Result.Success();
    }

    public Result Validar()
    {
        var result = Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(Nome), Nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarEnum(nameof(Tipo), Tipo)
        );

        return result;
    }
}
