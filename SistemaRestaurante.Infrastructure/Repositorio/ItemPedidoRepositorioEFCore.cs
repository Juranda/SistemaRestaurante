using SistemaRestaurante.Application.Repositorios;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Infrastructure.Repositorio;

// Items são inseridos em cascata quando o Pedido é criado via PedidoRepositorioEFCore.
// CriarVariosAsync é no-op aqui para que o UseCase não precise saber qual implementação está ativa.
public class ItemPedidoRepositorioEFCore : IItemPedidoRepositorio
{
    public Task CriarVariosAsync(IEnumerable<ItemPedido> items)
        => Task.CompletedTask;
}
