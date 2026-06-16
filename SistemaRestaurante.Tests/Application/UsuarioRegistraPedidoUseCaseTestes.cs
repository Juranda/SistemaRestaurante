using Moq;
using SistemaRestaurante.Application.DTO.Pedidos;
using SistemaRestaurante.Application.Repositorios;
using SistemaRestaurante.Application.UseCases;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Tests.Application;

public class UsuarioRegistraPedidoUseCaseTests
{
    // ── Mocks ──────────────────────────────────────────────────────────────────
    private readonly Mock<IUsuarioRepositorio> _usuarioRepo = new();
    private readonly Mock<ISetorRepositorio> _setorRepo = new();
    private readonly Mock<IPedidoRepositorio> _pedidoRepo = new();
    private readonly Mock<IProdutoRepositorio> _produtoRepo = new();
    private readonly Mock<IItemPedidoRepositorio> _itemPedidoRepo = new();

    private UsuarioRegistraPedidoUseCase CriarUseCase() => new(
        _usuarioRepo.Object,
        _setorRepo.Object,
        _pedidoRepo.Object,
        _produtoRepo.Object,
        _itemPedidoRepo.Object
    );

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static RegistraPedido PedidoValido(int usuarioId = 1) => new(
        UsuarioId: usuarioId,
        NomeCliente: "Mesa VIP",
        NumeroMesa: 5,
        Items: [new RegistraItemPedido(ProdutoId: 10, Quantidade: 2)]
    );

    private static Usuario UsuarioValido(int setorId = 99) => Usuario.Criar(1, "Usuario Teste", "123456", setorId).Value!;

    private static Setor SetorQuePermiteCriar() => Setor.Criar(1, "SetorQuePermiteCriar", TipoSetor.CRIA_PEDIDO).Value!;

    private static Setor SetorQuePermiteCriarERealizar() => Setor.Criar(2, "SetorQuePermiteCriarERealizar", TipoSetor.CRIA_REALIZA_PEDIDO).Value!;

    private static Setor SetorSemPermissao() => Setor.Criar(1, "SetorSemPermissao", TipoSetor.REALIZA_PEDIDO).Value!;

    // ── Testes: caminho feliz ──────────────────────────────────────────────────

    [Fact]
    public async Task Execute_PedidoValido_RetornaSucesso()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(99)).ReturnsAsync(SetorQuePermiteCriar());
        _produtoRepo.Setup(r => r.ObterTodosIndisponiveisAsync()).ReturnsAsync([]);

        var result = await CriarUseCase().Execute(PedidoValido());

        Assert.False(result.IsError);
        _pedidoRepo.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task Execute_SetorCriaRealiza_TambemPermiteCriarPedido()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(99)).ReturnsAsync(SetorQuePermiteCriarERealizar());
        _produtoRepo.Setup(r => r.ObterTodosIndisponiveisAsync()).ReturnsAsync([]);

        var result = await CriarUseCase().Execute(PedidoValido());

        Assert.False(result.IsError);
    }

    // ── Testes: usuário / setor ────────────────────────────────────────────────

    [Fact]
    public async Task Execute_UsuarioNaoExiste_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Usuario?)null);

        var result = await CriarUseCase().Execute(PedidoValido());

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosUsuario.UsuarioNaoExiste());
        _pedidoRepo.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_SetorNaoExiste_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(99)).ReturnsAsync((Setor?)null);

        var result = await CriarUseCase().Execute(PedidoValido());

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosSetor.SetorNaoExiste());
        _pedidoRepo.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_SetorSemPermissaoDeCriarPedido_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(99)).ReturnsAsync(SetorSemPermissao());

        var result = await CriarUseCase().Execute(PedidoValido());

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosSetor.SetorNaoPodeCriarPedido());
        _pedidoRepo.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    // ── Testes: produtos indisponíveis ────────────────────────────────────────

    [Fact]
    public async Task Execute_PedidoComProdutoIndisponivel_LancaException()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(99)).ReturnsAsync(SetorQuePermiteCriar());

        // ProdutoId 10 está indisponível — mesmo id do item no PedidoValido()
        _produtoRepo.Setup(r => r.ObterTodosIndisponiveisAsync())
                    .ReturnsAsync([Produto.Criar(10, "Produto", 10.0, 1, true).Value!]);

        await Assert.ThrowsAsync<Exception>(() =>
            CriarUseCase().Execute(PedidoValido()));

        _pedidoRepo.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_PedidoComProdutoDiferenteDoIndisponivel_NaoLancaException()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(99)).ReturnsAsync(SetorQuePermiteCriar());

        // ProdutoId 99 indisponível, mas o pedido tem ProdutoId 10 — sem conflito
        _produtoRepo.Setup(r => r.ObterTodosIndisponiveisAsync())
                    .ReturnsAsync([Produto.Criar(99, "Produto", 10.0, 1, true).Value!]);

        var result = await CriarUseCase().Execute(PedidoValido());

        Assert.False(result.IsError);
    }

    // ── Testes: validação dos itens ───────────────────────────────────────────

    [Fact]
    public async Task Execute_ItemComQuantidadeInvalida_RetornaErrosSemCriarPedido()
    {
        var pedidoComItemInvalido = new RegistraPedido(
            UsuarioId: 1,
            NomeCliente: "Mesa 3",
            NumeroMesa: 3,
            Items: [new RegistraItemPedido(ProdutoId: 10, Quantidade: 0)] // quantidade inválida
        );

        var result = await CriarUseCase().Execute(pedidoComItemInvalido);

        Assert.True(result.IsError);
        _usuarioRepo.Verify(r => r.ObterPorIdAsync(It.IsAny<int>()), Times.Never);
        _pedidoRepo.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }
}