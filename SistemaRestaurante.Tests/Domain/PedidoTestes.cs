using SistemaRestaurante.Domain.Entidades;
using SistemaRestaurante.Domain.Errors;

namespace SistemaRestaurante.Tests.Domain;

public class PedidoTestes
{
    [Fact]
    public void PedidoInvalido_DeveRetornar_Erro()
    {
        Result<Pedido> result = Pedido.Criar(0, null, -1, []);

        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
    }
}