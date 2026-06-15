using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using SistemaRestaurante.Application.DTO.Autenticacao;
using SistemaRestaurante.Application.InterfacesDeServicos;

namespace SistemaRestaurante.Infrastructure.Autenticacao;

public class CustomAuthStateProvider : AuthenticationStateProvider, IAutenticacaoServico
{
    private ClaimsPrincipal usuarioAtual = new(new ClaimsIdentity());

    public async Task LogarUsuario(LogarUsuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim("SetorId", usuario.SetorId.ToString()),
            new Claim("SetorNome", usuario.SetorNome.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        usuarioAtual = new(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public Task DesconectarUsuario()
    {
        usuarioAtual = new(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }


    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(usuarioAtual));
    }
}
