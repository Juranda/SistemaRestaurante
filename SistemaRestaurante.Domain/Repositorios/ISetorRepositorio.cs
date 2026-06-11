using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Domain.Repositorios;

public interface ISetorRepositorio
{
    public Task<IEnumerable<Setor>> ObterTodosAsync();
    public Task<Setor> CriarAsync(Setor setor);
    public Task<Setor> AtualizarAsync(Setor setor);
    public Task<bool> RemoverAsync(int id);
}
