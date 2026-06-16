using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Domain.Repositorios;

public interface IProdutoRepositorio
{
    public Task<Produto?> ObterPorIdAsync(int id);
    public Task<IEnumerable<Produto>> ObterTodosAsync(int page, int pageSize);
    public Task<IEnumerable<Produto>> ObterTodosIndisponiveisAsync();
    public Task<IEnumerable<Produto>> ObterTodosDisponiveisAsync();
    public Task<Produto> CriarAsync(Produto produto);
    public Task<Produto> AtualizarAsync(Produto produto);
    public Task<bool> RemoverAsync(int id);
}
