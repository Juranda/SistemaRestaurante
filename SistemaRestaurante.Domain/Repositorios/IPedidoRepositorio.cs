using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Domain.Repositorios;

public interface IPedidoRepositorio
{
    public Task<Pedido?> ObterPorIdAsync(int id);
    public Task<IEnumerable<Pedido>> ObterTodosPedidosAteHorarioAsync(DateTime horario);
    public Task<IEnumerable<Pedido>> ObterPedidosFinalizadosAsync();
    public Task<Pedido> CriarAsync(Pedido pedido);
    public Task<Pedido> AtualizarAsync(Pedido pedido);
    public Task<bool> RemoverAsync(int id);
}
