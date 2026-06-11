using SistemaRestaurante.Domain;
using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    private const int MIN_CARACTERES_NOME = 5;
    private const int MAX_CARACTERES_NOME = 255;
    public double Preco { get; private set; }
    public int SetorPreparoId { get; set; }

    private Produto(int id, string nome, double preco, int setorPreparoId)
    {
        Id = id;
        Nome = nome;
        Preco = preco;
        SetorPreparoId = setorPreparoId;
    }

    public static Result<Produto> Criar(int id, string nome, double preco, int setorPreparoId)
    {
        var result = Result.All(
            Validacoes.ValidarNumero(nameof(id), id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(nome), nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(preco), preco, 0.01, double.MaxValue),
            Validacoes.ValidarNumero(nameof(setorPreparoId), setorPreparoId, 1, int.MaxValue)
        );

        if (result.IsError)
        {
            return (Result<Produto>)result;
        }

        return new Produto(id, nome, preco, setorPreparoId);
    }

    public Result Atualizar(string nome, double preco, int setorPreparoId)
    {
        var result = Result.All(
            Validacoes.ValidarTexto(nameof(nome), nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(preco), preco, 0, double.MaxValue),
            Validacoes.ValidarNumero(nameof(setorPreparoId), setorPreparoId, 1, int.MaxValue)
        );

        if (result.IsError)
        {
            return (Result<Produto>)result;
        }

        Nome = nome;
        SetorPreparoId = setorPreparoId;

        return Result.Success();
    }

    internal Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(Nome), Nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(Preco), Preco, 0, double.MaxValue)
        );
    }
}
