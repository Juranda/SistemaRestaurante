using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using SistemaRestaurante.Application.DTO.Autenticacao;
using SistemaRestaurante.Application.InterfacesDeServicos;

namespace SistemaRestaurante.Infrastructure.Autenticacao;

public class CustomAuthStateProvider(
    IHttpContextAccessor httpContextAccessor,
    NavigationManager navigationManager
) : AuthenticationStateProvider, IAutenticacaoServico
{
    private ClaimsPrincipal _usuarioAtual = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Durante o request HTTP inicial (pre-render), HttpContext.User já tem as claims
        // do cookie. Sincroniza para o campo in-memory, que será usado nas interações
        // SignalR subsequentes quando HttpContext for null.
        var httpUser = httpContextAccessor.HttpContext?.User;
        if (httpUser?.Identity?.IsAuthenticated == true)
            _usuarioAtual = httpUser;

        return Task.FromResult(new AuthenticationState(_usuarioAtual));
    }

    public Task LogarUsuario(LogarUsuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim("SetorId", usuario.SetorId.ToString()),
            new Claim("SetorNome", usuario.SetorNome),
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        _usuarioAtual = new(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }

    public Task DesconectarUsuario()
    {
        _usuarioAtual = new(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        // forceLoad: true sai do circuit SignalR e faz um request HTTP normal,
        // onde o Razor Page pode chamar SignOutAsync e apagar o cookie.
        navigationManager.NavigateTo("/auth/sair", forceLoad: true);
        return Task.CompletedTask;
    }
}
