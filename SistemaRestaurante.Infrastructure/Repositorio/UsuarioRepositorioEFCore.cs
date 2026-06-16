using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Application.Repositorios;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class UsuarioRepositorioEFCore(EFCoreContext context) : IUsuarioRepositorio
{
    public async Task<Usuario?> ObterPorIdAsync(int id)
        => await context.Usuarios.FindAsync(id);

    public async Task<Usuario?> ObterUsuarioAsync(string nome)
        => await context.Usuarios.FirstOrDefaultAsync(u => u.Nome == nome);

    public async Task<bool> ExisteAsync(string nome)
        => await context.Usuarios.AnyAsync(u => u.Nome == nome);

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        context.Usuarios.Update(usuario);
        await context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var usuario = await context.Usuarios.FindAsync(id);
        if (usuario is null) return false;
        context.Usuarios.Remove(usuario);
        await context.SaveChangesAsync();
        return true;
    }
}
