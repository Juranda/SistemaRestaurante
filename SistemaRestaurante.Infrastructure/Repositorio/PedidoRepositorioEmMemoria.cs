using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Infrastructure.Repositorio;

public class PedidoRepositorioEmMemoria : IPedidoRepositorio
{
    private readonly List<Pedido> _pedidos = [];
    // public PedidoRepositorioEmMemoria()
    // {
    //     List<string> nomes = ["Ana", "Bruno", "Carlos", "Daniel", "Esther", "Felipe"];
    //     List<(int, string, double, int)> produtos = [
    //         (1, "Feijoada", 30.20, 1),
    //         (2, "Baião de Dois", 15.49, 1),
    //         (3, "Virado à Paulista", 20.0, 1),
    //         (4, "Tacacá", 10.0, 1),
    //         (5, "Moqueca", 8.50, 1),
    //         (6, "Spaghetti alla Carbonara", 42.90, 1),
    //         (7, "Ratatouille", 60.25, 1)
    //     ];

    //     List<ItemPedido> GetRandomItems(int pedidoId)
    //     {
    //         int quantidadeItens = Random.Shared.Next(1, 6);

    //         List<ItemPedido> itens = [];

    //         for (int i = 0; i < quantidadeItens; i++)
    //         {
    //             var (id, nome, preco, setor) = produtos[i];
    //             Result<Produto> result = Produto.Criar(id, nome, preco, setor, true);

    //             if (result.IsError)
    //             {
    //                 Console.WriteLine(result.ToString());
    //                 throw new Exception("Erro ao criar pedido");
    //             }

    //             Produto produto = result.Value!;

    //             itens.Add(
    //                 ItemPedido.Criar(
    //                     pedidoId,
    //                     (int)Random.Shared.NextInt64(1, 20),
    //                     produto.Id)
    //                 .Value!);
    //         }

    //         return itens;
    //     }

    //     for (int i = 0; i < 20; i++)
    //     {
    //         Result<Pedido> pedidoResult = Pedido.Criar(i + 1, nomes[i % nomes.Count], i + 1, GetRandomItems(i + 1), DateTime.Now.AddMinutes(GetRandomMinutes(i)));

    //         if (pedidoResult.IsError)
    //         {
    //             throw new Exception("Erro ao criar pedido: " + pedidoResult.ToString());
    //         }

    //         Pedido pedido = pedidoResult.Value!;

    //         if (i < 10)
    //         {
    //             foreach (var item in pedido.ItemsPedido)
    //             {
    //                 var result = item.AlterarStatus(StatusPedido.ENTREGUE);

    //                 if(result.IsError)
    //                 {
    //                     throw new Exception(result.Errors![0].Message);
    //                 }
    //             }
    //         }
    //         else if (i < 15)
    //         {
    //             foreach (var item in pedido.ItemsPedido)
    //             {
    //                 var result = item.AlterarStatus(StatusPedido.PRONTO);

    //                 if(result.IsError)
    //                 {
    //                     throw new Exception(result.Errors![0].Message);
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             foreach (var item in pedido.ItemsPedido)
    //             {
    //                 var result = item.AlterarStatus(StatusPedido.EM_PREPARO);

    //                 if(result.IsError)
    //                 {
    //                     throw new Exception(result.Errors![0].Message);
    //                 }
    //             }
    //         }


    //         _pedidos.Add(pedido);
    //     }

    //     static long GetRandomMinutes(int i)
    //     {
    //         long minutes = (i + 1) * Random.Shared.NextInt64(-121, -5);
    //         //Console.WriteLine("Minutos aleatorios: {0}", minutes);

    //         return minutes;
    //     }
    // }

    public Task<Pedido> CriarAsync(Pedido pedido)
    {
        int pedidoId = _pedidos.Count + 1;

        var items = pedido.ItemsPedido.ToList();
        foreach (var item in items)
            item.AtualizarPedidoId(pedidoId);

        Result<Pedido> result = Pedido.Criar(pedidoId, pedido.NomeCliente, pedido.NumeroMesa, items, DateTime.Now);

        if (result.IsError)
            throw new Exception($"Pedido inválido ao criar: {result}");

        Pedido pedidoNovo = result.Value!;
        _pedidos.Add(pedidoNovo);

        return Task.FromResult(pedidoNovo);
    }

    public Task<Pedido> AtualizarAsync(Pedido pedido)
    {
        var index = _pedidos.FindIndex(x => x.Id == pedido.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Pedido {pedido.Id} não encontrado.");
        }

        _pedidos[index] = pedido;

        return Task.FromResult(pedido);
    }
    public Task<Pedido?> ObterPorIdAsync(int id)
    {
        var pedido = _pedidos.FirstOrDefault(
            x => x.Id == id);

        return Task.FromResult(pedido);
    }

    public Task<IEnumerable<Pedido>> ObterTodosPedidosAteHorarioAsync(DateTime horario)
    {
        List<Pedido> pedidos = [];

        foreach (var pedido in _pedidos)
        {
            if (pedido.Timestamp <= horario)
            {
                pedidos.Add(pedido);
            }
        }

        return Task.FromResult(pedidos.AsEnumerable());
    }

    public Task<IEnumerable<Pedido>> ObterPedidosFinalizadosAsync()
    {
        var finalizados = _pedidos
            .Where(p => p.Status == StatusPedido.ENTREGUE)
            .OrderByDescending(p => p.Timestamp);

        return Task.FromResult(finalizados.AsEnumerable());
    }

    public Task<bool> RemoverAsync(int id)
    {
        var pedido = _pedidos.FirstOrDefault(x => x.Id == id);

        if (pedido is null)
        {
            return Task.FromResult(false);
        }

        _pedidos.Remove(pedido);

        return Task.FromResult(true);
    }
}