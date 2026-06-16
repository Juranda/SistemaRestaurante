using SistemaRestaurante.Application.Repositorios;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Application.UseCases;

public class HistoricoPedidosUseCase(IPedidoRepositorio pedidoRepositorio)
{
    public Task<IEnumerable<Pedido>> Execute() =>
        pedidoRepositorio.ObterPedidosFinalizadosAsync();
}
