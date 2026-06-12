namespace SistemaRestaurante.Domain.Errors;

public static class ErrosPedido
{
    public static Result AlteraStatusPedidoFinalizado() => new Error("PED-DOM-001", "O pedido já foi finalizado. O status não pode ser alterado.", ErrorTypes.Conflict);
    public static Result AlterarQuantidadeDeItemInexistente() => new Error("PED-DOM-002", "O pedido não possui este item. A quantidade não pode ser alterada.", ErrorTypes.Conflict);
}