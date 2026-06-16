using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Application.Repositorios;

public interface IUsuarioRepositorio
{
    public Task<Usuario?> ObterPorIdAsync(int id);
    public Task<Usuario?> ObterUsuarioAsync(string nome);
    public Task<Usuario> CriarAsync(Usuario usuario);
    public Task<Usuario> AtualizarAsync(Usuario usuario);
    public Task<bool> RemoverAsync(int id);
    public Task<bool> ExisteAsync(string nome);
}