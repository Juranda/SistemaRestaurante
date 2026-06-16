using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Pedido
{
    public int Id { get; private set; }
    public string NomeCliente { get; private set; }
    public const int MIN_CARACTERES_NOME_CLIENTE = 2;
    public const int MAX_CARACTERES_NOME_CLIENTE = 50;
    public int NumeroMesa { get; private set; }
    public DateTime Timestamp { get; private set; }
    public IReadOnlyList<ItemPedido> ItemsPedido => itemsPedido.AsReadOnly();
    private List<ItemPedido> itemsPedido { get; set; }

    public bool Pronto => itemsPedido.All(x => x.Status == StatusPedido.PRONTO);
    public int Quantidade => itemsPedido.Select(x => x.Quantidade).Aggregate((prev, curr) => prev += curr);

    private Pedido(int id, string nomeCliente, int numeroMesa, List<ItemPedido> itemsPedido, DateTime timestamp)
    {
        Id = id;
        NomeCliente = nomeCliente;
        NumeroMesa = numeroMesa;
        this.itemsPedido = itemsPedido;
        Timestamp = timestamp;
    }

    public StatusPedido Status
    {
        get
        {
            if(itemsPedido.Count == 0)
            {
                return StatusPedido.EM_PREPARO;
            }

            if (itemsPedido.All(x => x.Status == StatusPedido.ENTREGUE))
            {
                return StatusPedido.ENTREGUE;
            }

            if (itemsPedido.All(x => x.Status == StatusPedido.PRONTO))
            {
                return StatusPedido.PRONTO;
            }

            if(itemsPedido.Any(x => x.Status == StatusPedido.EM_PREPARO) == false)
            {
                return StatusPedido.PRONTO;
            }

            return StatusPedido.EM_PREPARO;
        }
    }

    public static Result<Pedido> Criar(int id, string nomeCliente, int numeroMesa, List<ItemPedido> itemsPedido, DateTime timestamp)
    {
        var pedido = new Pedido(id, nomeCliente, numeroMesa, itemsPedido, timestamp);
        var result = pedido.Validar();

        if (result.IsError)
        {
            return Result<Pedido>.FromResult(result);
        }

        return pedido;
    }

    public Result Validar()
    {
        Result validacaoItems;

        if (itemsPedido.Count == 0)
        {
            validacaoItems = ErrosPedido.PedidoSemItems();
        }
        else
        {
            validacaoItems = itemsPedido.Select(x => x.Validar()).Aggregate((prev, curr) => prev.CombineResult(curr))!;
        }

        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(NomeCliente), NomeCliente, MIN_CARACTERES_NOME_CLIENTE, MAX_CARACTERES_NOME_CLIENTE),
            Validacoes.ValidarNumero(nameof(NumeroMesa), NumeroMesa, 1, int.MaxValue),
            validacaoItems
        );
    }

    public Result AlterarQuantidade(int produtoId, int quantidade)
    {
        var item = itemsPedido.FirstOrDefault(x => x.ProdutoId == produtoId);

        if (item is null)
        {
            Result<ItemPedido> result = ItemPedido.Criar(Id, quantidade, produtoId);

            if(result.IsError)
            {
                return result;
            }

            itemsPedido.Add(result.Value!);

            return Result.Success();
        }

        if (quantidade == 0)
        {
            itemsPedido.Remove(item);
        }

        return item.AlterarQuantidade(quantidade);
    }

    public Result AvancarStatusItem(int produtoId)
    {
        ItemPedido? item = ItemsPedido.FirstOrDefault(x => x.ProdutoId == produtoId);

        if (item is null)
        {
            return ErrosPedido.AlterarStatusItemPedidoInexistente();
        }

        return item.AvancarStatus();
    }
}
