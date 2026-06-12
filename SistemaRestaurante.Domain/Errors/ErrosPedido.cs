

namespace SistemaRestaurante.Domain.Errors;

public static class ErrosPedido
{
    public static Error AlteraStatusPedidoFinalizado() => new Error("PED-DOM-001", "O pedido já foi finalizado. O status não pode ser alterado.", ErrorTypes.Conflict);
    public static Error AlterarQuantidadeDeItemInexistente() => new Error("PED-DOM-002", "O pedido não possui este item. A quantidade não pode ser alterada.", ErrorTypes.NotFound);
    public static Error AlterarParaStatusInvalido() => new Error("PED-DOM-003", "O item do pedido pode ser alterado para este estado.", ErrorTypes.Conflict);
    public static Error PedidoSemItems() => new Error("PED-DOM-004", "O pedido não possui items.", ErrorTypes.Validation);
    public static Error PedidoNaoExiste() => new Error("PED-INF-001", "Pedido não existe", ErrorTypes.NotFound);
}