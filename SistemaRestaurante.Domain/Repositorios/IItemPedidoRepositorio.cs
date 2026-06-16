using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Domain.Repositorios;

public interface IItemPedidoRepositorio
{
    Task CriarVariosAsync(IEnumerable<ItemPedido> items);
}
