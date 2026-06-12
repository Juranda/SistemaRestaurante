using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;
using Xunit.Abstractions;

namespace SistemaRestaurante.Tests.Domain;

public class PedidoTestes
{
    public readonly ITestOutputHelper output;

    public PedidoTestes(ITestOutputHelper testOutputHelper)
    {
        output = testOutputHelper;
    }

    private static Result<Pedido> GerarResultCriarPedidoValido()
    {
        return Pedido.Criar(1, "Felipe", 2, GerarItemsPedidoValidos());
    }

    private static List<ItemPedido> GerarItemsPedidoValidos()
    {
        List<ItemPedido> itemsPedido = [];
        Result<ItemPedido> resultItem;

        for (int i = 1; i < 6; i++)
        {
            resultItem = ItemPedido.Criar(1, 5, i);

            if (resultItem.IsError)
            {
                throw new Exception("Algo de errado ao criar o item do pedido");
            }

            itemsPedido.Add(resultItem.Value!);
        }

        return itemsPedido;
    }

    private static Pedido GerarPedidoValido()
    {
        Result<Pedido> result = GerarResultCriarPedidoValido();

        if (result.IsError)
        {
            throw new Exception("Algo de errado aconteceu a criar um pedido valido");
        }

        return result.Value!;
    }

    [Fact]
    public void CriarPedidoValido_NaoDeveRetornar_Erro()
    {
        Result<Pedido> result = GerarResultCriarPedidoValido();

        Assert.True(result.IsSuccess);
        Assert.Null(result.Errors);
        Assert.NotNull(result.Value);

        output.WriteLine(result.ToString());
    }

    [Fact]
    public void CriarPedidoInvalido_DeveRetornar_Erro()
    {
        Result<Pedido> result = Pedido.Criar(0, null, -1, []);
        Result<Pedido> result2 = Pedido.Criar(1, "", 10, GerarItemsPedidoValidos());

        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);

        Assert.True(result2.IsError);
        Assert.NotNull(result2.Errors);
        Assert.NotEmpty(result2.Errors);

        output.WriteLine(result.ToString());
        output.WriteLine(result2.ToString());
    }

    [Fact]
    public void AlterarQuantidadeValidaPedido_NaoDeveRetornar_Erro()
    {
        Pedido pedido = GerarPedidoValido();
        ItemPedido item = pedido.ItemsPedido[0];

        Result resultAltera = pedido.AlterarQuantidade(item.ProdutoId, 3);

        Assert.True(resultAltera.IsSuccess);
        Assert.Null(resultAltera.Errors);

        output.WriteLine(resultAltera.ToString());
    }


    [Fact]
    public void AlterarQuantidadeInvalidaPedido_DeveRetornar_Erro()
    {
        Pedido pedido = GerarPedidoValido();
        ItemPedido item = pedido.ItemsPedido[0];

        Result resultAltera = pedido.AlterarQuantidade(item.ProdutoId, -1);

        Assert.True(resultAltera.IsError);
        Assert.NotNull(resultAltera.Errors);
        Assert.NotEmpty(resultAltera.Errors);

        output.WriteLine(resultAltera.ToString());
    }
    
    [Fact]
    public void Status_DeveRetornar_StatusCorreto()
    {
        Pedido pedido = GerarPedidoValido();
        Assert.True(pedido.Status == StatusPedido.EM_PREPARO);

        ItemPedido primeiroItem = pedido.ItemsPedido[0];
        primeiroItem.AvancarStatus();

        Assert.True(pedido.Status == StatusPedido.EM_PREPARO);

        foreach(var item in pedido.ItemsPedido)
        {
            if(item.ProdutoId != primeiroItem.ProdutoId)
                pedido.AvancarStatusItem(item.ProdutoId);
        }

        Assert.True(pedido.Status == StatusPedido.PRONTO);

        primeiroItem.AvancarStatus();

        Assert.True(pedido.Status == StatusPedido.PRONTO);

        foreach(var item in pedido.ItemsPedido)
        {
            if(item.ProdutoId != primeiroItem.ProdutoId)
                pedido.AvancarStatusItem(item.ProdutoId);
        }

        Assert.True(pedido.Status == StatusPedido.ENTREGUE);
    }
}