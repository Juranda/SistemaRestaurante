using Moq;
using SistemaRestaurante.Application.Repositorios;
using SistemaRestaurante.Application.UseCases;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Tests.Application;

public class HistoricoPedidosUseCaseTestes
{
    private readonly Mock<IPedidoRepositorio> _pedidoRepo = new();

    private HistoricoPedidosUseCase CriarUseCase() => new(_pedidoRepo.Object);

    private static Pedido CriarPedidoFinalizado(int id)
    {
        var item = ItemPedido.Criar(id, 1, 1).Value!;
        item.AvancarStatus(); // EM_PREPARO → PRONTO
        item.AvancarStatus(); // PRONTO → ENTREGUE
        return Pedido.Criar(id, "Cliente", 1, [item], DateTime.Now).Value!;
    }

    [Fact]
    public async Task Execute_RetornaListaDoRepositorio()
    {
        var pedidos = new List<Pedido>
        {
            CriarPedidoFinalizado(1),
            CriarPedidoFinalizado(2),
        };
        _pedidoRepo.Setup(r => r.ObterPedidosFinalizadosAsync()).ReturnsAsync(pedidos);

        var result = await CriarUseCase().Execute();

        Assert.Equal(2, result.Count());
        _pedidoRepo.Verify(r => r.ObterPedidosFinalizadosAsync(), Times.Once);
    }

    [Fact]
    public async Task Execute_QuandoNaoHaPedidos_RetornaVazio()
    {
        _pedidoRepo.Setup(r => r.ObterPedidosFinalizadosAsync()).ReturnsAsync([]);

        var result = await CriarUseCase().Execute();

        Assert.Empty(result);
    }
}
