using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Domain.Repositorios;

public interface IMesaRepositorio
{
    public Task<Mesa> ObterPorIdAsync(int id);
    public Task<IEnumerable<Mesa>> ObterTodosAsync();
    public Task<Mesa> CriarAsync(Mesa mesa);
    public Task<Mesa> AtualizarAsync(Mesa mesa);
    public Task<bool> RemoverAsync(int id);
}
