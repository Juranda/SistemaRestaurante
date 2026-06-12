using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Pedido
{
    public int Id { get; private set; }
    public string NomeCliente { get; private set; }
    public const int MIN_CARACTERES_NOME = 5;
    public const int MAX_CARACTERES_NOME = 50;
    public int NumeroMesa { get; private set; }
    public IReadOnlyList<ItemPedido> ItemsPedido => itemsPedido.AsReadOnly();
    private List<ItemPedido> itemsPedido { get; set; }

    public bool Pronto => itemsPedido.All(x => x.Status == StatusPedido.PRONTO);

    private Pedido(int id, string nomeCliente, int numeroMesa, List<ItemPedido> itemsPedido)
    {
        Id = id;
        NomeCliente = nomeCliente;
        NumeroMesa = numeroMesa;
        this.itemsPedido = itemsPedido;
    }

    public StatusPedido Status()
    {
        if(itemsPedido.Count == 0)
        {
            return StatusPedido.ENTREGUE;
        }

        if (itemsPedido.All(x => x.Status == StatusPedido.EM_PREPARO))
        {
            return StatusPedido.EM_PREPARO;
        }

        if (itemsPedido.All(x => x.Status == StatusPedido.PRONTO))
        {
            return StatusPedido.PRONTO;
        }

        if (itemsPedido.All(x => x.Status == StatusPedido.ENTREGUE))
        {
            return StatusPedido.ENTREGUE;
        }

        throw new Exception("Status inválido");
    }

    public static Result<Pedido> Criar(int id, string nomeCliente, int numeroMesa, List<ItemPedido> itemsPedido)
    {
        var pedido = new Pedido(id, nomeCliente, numeroMesa, itemsPedido);
        var result = pedido.Validar();

        if (result.IsError)
        {
            return (Result<Pedido>)result;
        }

        return pedido;
    }

    public Result Validar()
    {
        Result validacoes = itemsPedido.Select(x => x.Validar()).Aggregate((prev, curr) => prev.CombineResult(curr))!;
        
        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(NomeCliente), NomeCliente, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(NumeroMesa), NumeroMesa, 1, int.MaxValue),
            validacoes
        );
    }

    public Result AlterarQuantidade(int produtoId, int quantidade)
    {
        var item = itemsPedido.FirstOrDefault(x => x.ProdutoId == produtoId);

        if(item is null)
        {
            return ErrosPedido.AlterarQuantidadeDeItemInexistente();
        }

        if(quantidade == 0)
        {
            itemsPedido.Remove(item);
        }

        return item.AlterarQuantidade(quantidade);
    }
}
