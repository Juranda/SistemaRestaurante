namespace SistemaRestaurante.Domain.Entidades;

public enum TipoSetor
{
    CRIA_PEDIDO = 1,
    REALIZA_PEDIDO = 2,
    CRIA_REALIZA_PEDIDO = CRIA_PEDIDO | REALIZA_PEDIDO
}
