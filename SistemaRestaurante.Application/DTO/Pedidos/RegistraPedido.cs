namespace SistemaRestaurante.Application.DTO.Pedidos;

public record RegistraPedido(
    int UsuarioId,
    string NomeCliente,
    int NumeroMesa,
    List<RegistraItemPedido> Items
);
