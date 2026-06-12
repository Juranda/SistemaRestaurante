using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public const int MIN_CARACTERES_NOME = 5;
    public const int MAX_CARACTERES_NOME = 255;
    public double Preco { get; private set; }
    public int SetorPreparoId { get; set; }
    public bool Disponivel { get; set; }

    private Produto(int id, string nome, double preco, int setorPreparoId)
    {
        Id = id;
        Nome = nome;
        Preco = preco;
        SetorPreparoId = setorPreparoId;
    }

    public static Result<Produto> Criar(int id, string nome, double preco, int setorPreparoId)
    {
        var produto = new Produto(id, nome, preco, setorPreparoId);
        var result = produto.Validar();

        if (result.IsError)
        {
            return Result<Produto>.FromResult(result);
        }

        return produto;
    }

    public Result Atualizar(string nome, double preco, int setorPreparoId)
    {
        var produto = new Produto(Id, nome, preco, setorPreparoId);
        var result = produto.Validar();

        if (result.IsError)
        {
            return (Result<Produto>)result;
        }

        Nome = nome;
        Preco = preco;
        SetorPreparoId = setorPreparoId;

        return Result.Success();
    }

    public Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(Nome), Nome, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(Preco), Preco, 0, double.MaxValue)
        );
    }
}
