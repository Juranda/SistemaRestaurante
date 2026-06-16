using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class ProdutoRepositorioEFCore(EFCoreContext context) : IProdutoRepositorio
{
    public async Task<Produto?> ObterPorIdAsync(int id)
        => await context.Produtos.FindAsync(id);

    public async Task<IEnumerable<Produto>> ObterTodosAsync(int page, int pageSize)
        => await context.Produtos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<IEnumerable<Produto>> ObterTodosDisponiveisAsync()
        => await context.Produtos.Where(p => p.Disponivel).ToListAsync();

    public async Task<IEnumerable<Produto>> ObterTodosIndisponiveisAsync()
        => await context.Produtos.Where(p => !p.Disponivel).ToListAsync();

    public async Task<Produto> CriarAsync(Produto produto)
    {
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        return produto;
    }

    public async Task<Produto> AtualizarAsync(Produto produto)
    {
        context.Produtos.Update(produto);
        await context.SaveChangesAsync();
        return produto;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var produto = await context.Produtos.FindAsync(id);

        if (produto is null) return false;
        
        context.Produtos.Remove(produto);
        await context.SaveChangesAsync();
        return true;
    }
}
