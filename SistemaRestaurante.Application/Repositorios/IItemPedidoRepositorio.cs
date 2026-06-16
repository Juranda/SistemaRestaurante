using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Application.Repositorios;

public interface IItemPedidoRepositorio
{
    Task CriarVariosAsync(IEnumerable<ItemPedido> items);
}
