using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class ItemPedido
{
    public int Quantidade { get; private set; }
    public int ProdutoId { get; private set; }

    private ItemPedido(int quantidade, int produtoId)
    {
        Quantidade = quantidade;
        ProdutoId = produtoId;
    }

    public static Result<ItemPedido> Criar(int quantidade, int produtoId)
    {
        var item = new ItemPedido(quantidade, produtoId);
        var result = item.Validar();

        if (result.IsError)
        {
            return (Result<ItemPedido>)result;
        }

        return item;
    }

    public Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(Quantidade), Quantidade, 1, int.MaxValue),
            Validacoes.ValidarNumero(nameof(ProdutoId), ProdutoId, 1, int.MaxValue)
        );
    }
}
