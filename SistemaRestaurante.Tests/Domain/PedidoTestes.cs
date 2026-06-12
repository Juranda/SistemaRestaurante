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

    [Fact]
    public void PedidoInvalido_DeveRetornar_Erro()
    {
        Result<Pedido> result = Pedido.Criar(0, null, -1, []);

        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);

        output.WriteLine(result.ToString());
    }
}