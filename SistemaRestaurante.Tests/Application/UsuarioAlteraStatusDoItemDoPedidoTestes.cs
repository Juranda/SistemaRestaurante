using Moq;
using SistemaRestaurante.Application.UseCases;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Tests.Application;

public class UsuarioAlteraStatusDoItemDoPedidoTestes
{
    private readonly Mock<IUsuarioRepositorio> _usuarioRepo = new();
    private readonly Mock<ISetorRepositorio> _setorRepo = new();
    private readonly Mock<IPedidoRepositorio> _pedidoRepo = new();

    private const int USUARIO_ID = 1;
    private const int SETOR_ID = 99;
    private const int PEDIDO_ID = 10;
    private const int PRODUTO_ID = 5;

    private UsuarioAlteraStatusDoItemDoPedido CriarUseCase() =>
        new(_setorRepo.Object, _pedidoRepo.Object, _usuarioRepo.Object);

    private static Usuario UsuarioValido() =>
        Usuario.Criar(USUARIO_ID, "Funcionario", "123456", SETOR_ID).Value!;

    private static Setor SetorRealizaPedido() =>
        Setor.Criar(SETOR_ID, "Cozinha", TipoSetor.REALIZA_PEDIDO).Value!;

    private static Setor SetorCriaERealiza() =>
        Setor.Criar(SETOR_ID, "Geral", TipoSetor.CRIA_REALIZA_PEDIDO).Value!;

    private static Setor SetorSemPermissao() =>
        Setor.Criar(SETOR_ID, "Caixa", TipoSetor.CRIA_PEDIDO).Value!;

    private static Pedido PedidoComItem()
    {
        var item = ItemPedido.Criar(PEDIDO_ID, 2, PRODUTO_ID).Value!;
        return Pedido.Criar(PEDIDO_ID, "Mesa 1", 1, [item], DateTime.Now).Value!;
    }

    private static Pedido PedidoComItemPronto()
    {
        var item = ItemPedido.Criar(PEDIDO_ID, 2, PRODUTO_ID).Value!;
        item.AvancarStatus(); // EM_PREPARO → PRONTO
        return Pedido.Criar(PEDIDO_ID, "Mesa 1", 1, [item], DateTime.Now).Value!;
    }

    private static Pedido PedidoSemEsseProduto()
    {
        var item = ItemPedido.Criar(PEDIDO_ID, 2, 999).Value!; // produto diferente
        return Pedido.Criar(PEDIDO_ID, "Mesa 1", 1, [item], DateTime.Now).Value!;
    }

    // ── Caminho feliz ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Execute_DadosValidos_RetornaSucessoEAtualiza()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync(SetorRealizaPedido());
        _pedidoRepo.Setup(r => r.ObterPorIdAsync(PEDIDO_ID)).ReturnsAsync(PedidoComItem());

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.False(result.IsError);
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task Execute_SetorCriaERealiza_TambemPermiteAlterarStatus()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync(SetorCriaERealiza());
        _pedidoRepo.Setup(r => r.ObterPorIdAsync(PEDIDO_ID)).ReturnsAsync(PedidoComItem());

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.False(result.IsError);
    }

    // ── Validações de usuário / setor ──────────────────────────────────────────

    [Fact]
    public async Task Execute_UsuarioNaoExiste_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Usuario?)null);

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosUsuario.UsuarioNaoExiste());
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_SetorNaoExiste_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync((Setor?)null);

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosSetor.SetorNaoExiste());
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_SetorSemPermissaoDeRealizar_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync(SetorSemPermissao());

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosSetor.SetorNaoPodeAlterarSituacaoPedido());
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    // ── Validações de pedido / item ────────────────────────────────────────────

    [Fact]
    public async Task Execute_PedidoNaoExiste_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync(SetorRealizaPedido());
        _pedidoRepo.Setup(r => r.ObterPorIdAsync(PEDIDO_ID)).ReturnsAsync((Pedido?)null);

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosPedido.PedidoNaoExiste());
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ItemNaoExisteNoPedido_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync(SetorRealizaPedido());
        _pedidoRepo.Setup(r => r.ObterPorIdAsync(PEDIDO_ID)).ReturnsAsync(PedidoSemEsseProduto());

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.PRONTO);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosPedido.ItemDoPedidoNaoExiste());
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Execute_StatusRetrocede_RetornaErro()
    {
        _usuarioRepo.Setup(r => r.ObterPorIdAsync(USUARIO_ID)).ReturnsAsync(UsuarioValido());
        _setorRepo.Setup(r => r.ObterPorId(SETOR_ID)).ReturnsAsync(SetorRealizaPedido());
        _pedidoRepo.Setup(r => r.ObterPorIdAsync(PEDIDO_ID)).ReturnsAsync(PedidoComItemPronto());

        var result = await CriarUseCase().Execute(USUARIO_ID, PEDIDO_ID, PRODUTO_ID, StatusPedido.EM_PREPARO);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors!, e => e == ErrosPedido.AlterarParaStatusInvalido());
        _pedidoRepo.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }
}
