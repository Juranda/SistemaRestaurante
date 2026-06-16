using SistemaRestaurante.Application.DTO.Pedidos;
using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;
using SistemaRestaurante.Domain.Repositorios;

namespace SistemaRestaurante.Application.UseCases;

public class UsuarioRegistraPedidoUseCase(
    IUsuarioRepositorio usuarioRepositorio,
    ISetorRepositorio setorRepositorio,
    IPedidoRepositorio pedidoRepositorio,
    IProdutoRepositorio produtoRepositorio,
    IItemPedidoRepositorio itemPedidoRepositorio
)
{
    public async Task<Result> Execute(RegistraPedido registraPedido)
    {
        var (UsuarioId, NomeCliente, NumeroMesa, Items) = registraPedido;

        List<ItemPedido> itemsPedido = [];
        List<Error> erros = [];

        foreach (var item in Items)
        {
            // pedidoId=1 é um placeholder — será corrigido pelo repositório após a criação
            var resultItem = ItemPedido.Criar(1, item.Quantidade, item.ProdutoId);

            if (resultItem.IsError)
            {
                erros.AddRange(resultItem.Errors!);
                continue;
            }

            itemsPedido.Add(resultItem.Value!);
        }

        if (erros.Count > 0)
            return erros;

        var usuario = await usuarioRepositorio.ObterPorIdAsync(UsuarioId);

        if (usuario is null)
            return ErrosUsuario.UsuarioNaoExiste();

        var setorUsuario = await setorRepositorio.ObterPorId(usuario.SetorId);

        if (setorUsuario is null)
            return ErrosSetor.SetorNaoExiste();

        if (!setorUsuario.Tipo.HasFlag(TipoSetor.CRIA_PEDIDO))
            return ErrosSetor.SetorNaoPodeCriarPedido();

        IEnumerable<Produto> produtosIndisponiveis = await produtoRepositorio.ObterTodosIndisponiveisAsync();
        var idsProdutosIndisponiveis = produtosIndisponiveis.Select(x => x.Id);
        var idsProdutosNoPedido = itemsPedido.Select(x => x.ProdutoId);

        if (idsProdutosIndisponiveis.Intersect(idsProdutosNoPedido).Any())
            throw new Exception("Pedido com produtos indisponíveis");

        Result<Pedido> result = Pedido.Criar(1, NomeCliente, NumeroMesa, itemsPedido, DateTime.Now);

        if (result.IsError)
            return result;

        // Repositório atribui o ID real e atualiza os PedidoIds dos itens
        await pedidoRepositorio.CriarAsync(result.Value!);

        // Itens agora têm o PedidoId correto (atualizado pelo repositório via AtualizarPedidoId)
        await itemPedidoRepositorio.CriarVariosAsync(itemsPedido);

        return Result.Success();
    }
}
