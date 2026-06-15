using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Application.UseCases;

public class UsuarioAlteraStatusDoItemDoPedido(ISetorRepositorio setorRepositorio, IPedidoRepositorio pedidoRepositorio, IUsuarioRepositorio usuarioRepositorio)
{
    private readonly IUsuarioRepositorio usuarioRepositorio = usuarioRepositorio;
    private readonly ISetorRepositorio setorRepositorio = setorRepositorio;
    private readonly IPedidoRepositorio pedidoRepositorio = pedidoRepositorio;

    public async Task<Result> Execute(int usuarioId, int pedidoId, int produtoId, StatusPedido novoStatus)
    {
        Usuario? usuario = await usuarioRepositorio.ObterPorIdAsync(usuarioId);

        if (usuario is null)
        {
            return ErrosUsuario.UsuarioNaoExiste();
        }

        Setor? setorUsuario = await setorRepositorio.ObterPorId(usuario.SetorId);

        if (setorUsuario is null)
        {
            return ErrosSetor.SetorNaoExiste();
        }

        if (!setorUsuario.Tipo.HasFlag(TipoSetor.REALIZA_PEDIDO))
        {
            return ErrosSetor.SetorNaoPodeAlterarSituacaoPedido();
        }

        Pedido? pedido = await pedidoRepositorio.ObterPorIdAsync(pedidoId);

        if (pedido is null)
        {
            return ErrosPedido.PedidoNaoExiste();
        }

        ItemPedido? item = pedido.ItemsPedido.FirstOrDefault(x => x.PedidoId == pedidoId && x.ProdutoId == produtoId);

        if (item is null)
        {
            return ErrosPedido.ItemDoPedidoNaoExiste();
        }

        var result = item.AlterarStatus(novoStatus);

        if (result.IsError)
            return result;

        await pedidoRepositorio.AtualizarAsync(pedido);
        return Result.Success();
    }
}