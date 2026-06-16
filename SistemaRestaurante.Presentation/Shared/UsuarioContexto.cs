using System.Security.Claims;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Presentation.Shared;

public record UsuarioContexto(int Id, string Nome, int SetorId, string SetorNome, TipoSetor SetorTipo)
{
    public bool CriaPedido => SetorTipo.HasFlag(TipoSetor.CRIA_PEDIDO);
    public bool RealizaPedido => SetorTipo.HasFlag(TipoSetor.REALIZA_PEDIDO);

    public static UsuarioContexto? FromClaims(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true) return null;

        if (!int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)) return null;
        if (!int.TryParse(user.FindFirst("SetorId")?.Value, out var setorId)) return null;
        if (!int.TryParse(user.FindFirst("SetorTipo")?.Value, out var setorTipoInt)) return null;

        return new UsuarioContexto(
            id,
            user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            setorId,
            user.FindFirst("SetorNome")?.Value ?? string.Empty,
            (TipoSetor)setorTipoInt
        );
    }
}
