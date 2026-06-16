using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class ItemPedidoRepositorioEmMemoria : IItemPedidoRepositorio
{
    private readonly List<ItemPedido> _items = [];

    public Task CriarVariosAsync(IEnumerable<ItemPedido> items)
    {
        _items.AddRange(items);
        return Task.CompletedTask;
    }
}
