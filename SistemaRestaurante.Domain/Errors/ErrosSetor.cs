namespace SistemaRestaurante.Domain.Errors;

public static class ErrosSetor
{
    public static Error SetorNaoExiste() => new Error("SETO-APP-001", "Setor não existe", ErrorTypes.NotFound);
    public static Error SetorNaoPodeCriarPedido() => new Error("SETO-APP-002", "O setor atual não consegue criar pedidos", ErrorTypes.Unauthorized);
    public static Error SetorNaoPodeAlterarSituacaoPedido() => new Error("SETO-APP-003", "O setor atual não consegue alterar a situação de pedidos", ErrorTypes.Unauthorized);
    
}
