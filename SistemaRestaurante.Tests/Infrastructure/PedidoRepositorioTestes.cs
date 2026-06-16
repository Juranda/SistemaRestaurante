using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Infrastructure;
using SistemaRestaurante.Infrastructure.Repositorio;
using Xunit.Abstractions;

namespace SistemaRestaurante.Tests.Infrastructure;

public class PedidoRepositorioTestes : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly EFCoreContext _context;
    private readonly PedidoRepositorioEFCore _repositorio;
    private readonly List<int> _pedidosCriados = [];

    private const string ConnectionString =
        "Server=localhost,1433;Database=SistemaRestaurante;User Id=sa;Password=Senha@123456;TrustServerCertificate=True;";

    public PedidoRepositorioTestes(ITestOutputHelper output)
    {
        _output = output;

        var options = new DbContextOptionsBuilder<EFCoreContext>()
            .UseSqlServer(ConnectionString)
            .LogTo(
                msg => _output.WriteLine(msg),
                [DbLoggerCategory.Database.Command.Name],
                LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        _context = new EFCoreContext(options);
        _repositorio = new PedidoRepositorioEFCore(_context);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (_pedidosCriados.Count > 0)
            await _context.Pedidos
                .Where(p => _pedidosCriados.Contains(p.Id))
                .ExecuteDeleteAsync();

        await _context.DisposeAsync();
    }

    // --- helpers ---

    private static List<ItemPedido> Itens(params (int produtoId, int quantidade)[] itens)
        => itens.Select(i => ItemPedido.Criar(1, i.quantidade, i.produtoId).Value!).ToList();

    private async Task<Pedido> CriarPedido(string nomeCliente = "Teste", int mesa = 1,
        List<ItemPedido>? items = null, DateTime? timestamp = null)
    {
        items ??= Itens((1, 2));
        var pedido = Pedido.Criar(1, nomeCliente, mesa, items, timestamp ?? DateTime.Now).Value!;
        var criado = await _repositorio.CriarAsync(pedido);
        _pedidosCriados.Add(criado.Id);
        return criado;
    }

    // --- testes ---

    [Fact]
    public async Task CriarAsync_DeveAtribuirIdGeradoPeloBanco()
    {
        var pedido = await CriarPedido();

        Assert.True(pedido.Id > 0);
    }

    [Fact]
    public async Task CriarAsync_DeveAssociarItensAoPedidoCriado()
    {
        var pedido = await CriarPedido(items: Itens((1, 2), (3, 1)));

        Assert.Equal(2, pedido.ItemsPedido.Count);
        Assert.All(pedido.ItemsPedido, item => Assert.Equal(pedido.Id, item.PedidoId));
    }

    [Fact]
    public async Task CriarAsync_DoisPedidosSequenciais_DevemTerIdsDistintos()
    {
        var pedido1 = await CriarPedido("Cliente 1", mesa: 1);
        var pedido2 = await CriarPedido("Cliente 2", mesa: 2);

        Assert.NotEqual(pedido1.Id, pedido2.Id);
        Assert.All(pedido1.ItemsPedido, i => Assert.Equal(pedido1.Id, i.PedidoId));
        Assert.All(pedido2.ItemsPedido, i => Assert.Equal(pedido2.Id, i.PedidoId));
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarPedidoComItens()
    {
        var criado = await CriarPedido(items: Itens((1, 3)));

        var obtido = await _repositorio.ObterPorIdAsync(criado.Id);

        Assert.NotNull(obtido);
        Assert.Equal(criado.Id, obtido.Id);
        Assert.Single(obtido.ItemsPedido);
        Assert.Equal(3, obtido.ItemsPedido[0].Quantidade);
    }

    [Fact]
    public async Task ObterPorIdAsync_IdInexistente_DeveRetornarNull()
    {
        var resultado = await _repositorio.ObterPorIdAsync(int.MaxValue);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodosPedidosAteHorarioAsync_DeveRetornarApenasPedidosDentroDoIntervalo()
    {
        var agora = DateTime.Now;
        var pedidoPassado = await CriarPedido("Passado", mesa: 5, timestamp: agora.AddMinutes(-5));
        var pedidoFuturo  = await CriarPedido("Futuro",  mesa: 6, timestamp: agora.AddMinutes(+5));

        var resultado = (await _repositorio.ObterTodosPedidosAteHorarioAsync(agora)).ToList();

        Assert.Contains(resultado, p => p.Id == pedidoPassado.Id);
        Assert.DoesNotContain(resultado, p => p.Id == pedidoFuturo.Id);
    }
}
