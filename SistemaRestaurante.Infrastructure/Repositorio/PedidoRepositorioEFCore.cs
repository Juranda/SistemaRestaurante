using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class PedidoRepositorioEFCore(EFCoreContext context) : IPedidoRepositorio
{
    public async Task<Pedido?> ObterPorIdAsync(int id)
        => await context.Pedidos
            .Include(p => p.ItemsPedido)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Pedido>> ObterTodosPedidosAteHorarioAsync(DateTime horario)
        => await context.Pedidos
            .Include(p => p.ItemsPedido)
            .Where(p => p.Timestamp <= horario)
            .ToListAsync();

    public async Task<IEnumerable<Pedido>> ObterPedidosFinalizadosAsync()
        => await context.Pedidos
            .Include(p => p.ItemsPedido)
            .Where(p => p.ItemsPedido.All(i => i.Status == StatusPedido.ENTREGUE))
            .OrderByDescending(p => p.Timestamp)
            .ToListAsync();

    public async Task<Pedido> CriarAsync(Pedido pedido)
    {
        var items = pedido.ItemsPedido.ToList();

        context.Pedidos.Add(pedido);
        context.Entry(pedido).Property(p => p.Id).CurrentValue = 0;
        foreach (var item in items)
            context.Entry(item).State = EntityState.Detached;

        await context.SaveChangesAsync();

        foreach (var item in items)
        {
            item.AtualizarPedidoId(pedido.Id);
            context.Add(item);
        }
        await context.SaveChangesAsync();

        return pedido;
    }

    public async Task<Pedido> AtualizarAsync(Pedido pedido)
    {
        context.Pedidos.Update(pedido);
        await context.SaveChangesAsync();
        return pedido;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var pedido = await context.Pedidos
            .Include(p => p.ItemsPedido)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null) return false;
        
        context.Pedidos.Remove(pedido);
        await context.SaveChangesAsync();
        return true;
    }
}
