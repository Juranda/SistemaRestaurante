using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class SetorRepositorioEFCore(EFCoreContext context) : ISetorRepositorio
{
    public async Task<IEnumerable<Setor>> ObterTodosAsync()
        => await context.Setores.ToListAsync();

    public async Task<Setor?> ObterPorId(int id)
        => await context.Setores.FindAsync(id);

    public async Task<Setor> CriarAsync(Setor setor)
    {
        context.Setores.Add(setor);
        await context.SaveChangesAsync();
        return setor;
    }

    public async Task<Setor> AtualizarAsync(Setor setor)
    {
        context.Setores.Update(setor);
        await context.SaveChangesAsync();
        return setor;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var setor = await context.Setores.FindAsync(id);
        if (setor is null) return false;
        context.Setores.Remove(setor);
        await context.SaveChangesAsync();
        return true;
    }
}
