using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Application.UseCases;

public class HistoricoPedidosUseCase(IPedidoRepositorio pedidoRepositorio)
{
    public Task<IEnumerable<Pedido>> Execute() =>
        pedidoRepositorio.ObterPedidosFinalizadosAsync();
}
