using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Cardapio
{
    public int Id { get; set; }
    public List<Produto> Produtos { get; private set; }

    private Cardapio(int id, List<Produto> produtos)
    {
        Id = id;
        Produtos = produtos;
    }

    public static Result<Cardapio> Criar(int id, List<Produto> produtos)
    {
        var result = Validacoes.ValidarNumero(nameof(id), id, 1, int.MaxValue);

        if(result.IsError)
        {
            return (Result<Cardapio>)result;
        }

        return new Cardapio(id, produtos);
    }
}