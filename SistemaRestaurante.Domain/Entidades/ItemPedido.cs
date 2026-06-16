using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class ItemPedido
{
    public int PedidoId { get; private set; }
    public int ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public StatusPedido Status { get; private set; }

    private ItemPedido(int pedidoId, int quantidade, int produtoId)
    {
        PedidoId = pedidoId;
        Quantidade = quantidade;
        ProdutoId = produtoId;
        Status = StatusPedido.EM_PREPARO;
    }

    public static Result<ItemPedido> Criar(int pedidoId, int quantidade, int produtoId)
    {
        var item = new ItemPedido(pedidoId, quantidade, produtoId);
        var result = item.Validar();

        if (result.IsError)
        {
            return Result<ItemPedido>.FromResult(result);
        }

        return item;
    }

    public Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(PedidoId), PedidoId, 1, int.MaxValue),
            Validacoes.ValidarNumero(nameof(Quantidade), Quantidade, 1, int.MaxValue),
            Validacoes.ValidarNumero(nameof(ProdutoId), ProdutoId, 1, int.MaxValue)
        );
    }

    public Result AvancarStatus()
    {
        switch (Status)
        {
            case StatusPedido.EM_PREPARO:
                Status = StatusPedido.PRONTO;
                return Result.Success();
            case StatusPedido.PRONTO:
                Status = StatusPedido.ENTREGUE;
                return Result.Success();
            case StatusPedido.ENTREGUE:
                return ErrosPedido.AlteraStatusPedidoFinalizado();
            default:
                throw new Exception("Status do item inválido");
        }
    }

    public Result AlterarQuantidade(int quantidade)
    {
        var item = new ItemPedido(PedidoId, quantidade, ProdutoId);
        var result = item.Validar();

        if (result.IsError)
        {
            return result;
        }

        Quantidade = quantidade;
        return Result.Success();
    }

    public void AtualizarPedidoId(int pedidoId) => PedidoId = pedidoId;

    public Result AlterarStatus(StatusPedido status)
    {
        if (Status == status) return Result.Success();
        
        if (Status > status)
        {
            return ErrosPedido.AlterarParaStatusInvalido();
        }

        Status = status;
        return Result.Success();
    }
}
