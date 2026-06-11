using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Domain.Entidades;

public class Pedido
{
    public int Id { get; private set; }
    public string NomeCliente { get; private set; }
    private const int MIN_CARACTERES_NOME = 5;
    private const int MAX_CARACTERES_NOME = 50;
    public int MesaId { get; private set; }
    public StatusPedido Status { get; private set; }
    public List<ItemPedido> ItemsPedido { get; private set; }

    public bool Finalizado => Status == StatusPedido.ENTREGUE;

    private Pedido(int id, string nomeCliente, int mesaId, List<ItemPedido> itemsPedido)
    {
        Id = id;
        NomeCliente = nomeCliente;
        MesaId = mesaId;
        ItemsPedido = itemsPedido;
    }

    public static Result<Pedido> Criar(int id, string nomeCliente, int mesaId, List<ItemPedido> itemsPedido)
    {
        var result = Result.All(
            Validacoes.ValidarNumero(nameof(id), id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(nomeCliente), nomeCliente, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            itemsPedido.Select(x => x.Validar()).Aggregate((prev, curr) => prev.CombineResult(curr))
        );

        if (result.IsError)
        {
            return (Result<Pedido>)result;
        }

        return new Pedido(id, nomeCliente, mesaId, itemsPedido);
    }

    public Result AtualizarStatus()
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
                return PedidoErrors.AlteraStatusPedidoFinalizado();
            default:
                throw new Exception("Status do pedido inválido");
        }
    }

    public Result Validar()
    {
        return Result.All(
            Validacoes.ValidarNumero(nameof(Id), Id, 1, int.MaxValue),
            Validacoes.ValidarTexto(nameof(NomeCliente), NomeCliente, MIN_CARACTERES_NOME, MAX_CARACTERES_NOME),
            Validacoes.ValidarNumero(nameof(MesaId), MesaId, 1, int.MaxValue)
        );
    }
}

public static class PedidoErrors
{
    public static Result AlteraStatusPedidoFinalizado() => new Error("PED-DOM-001", "O pedido já foi finalizado. O status não pode ser alterado.", ErrorTypes.Conflict);
}