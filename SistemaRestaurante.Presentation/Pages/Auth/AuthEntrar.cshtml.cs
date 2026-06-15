using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaRestaurante.Application.DTO.Autenticacao;

namespace SistemaRestaurante.Presentation.Pages.Auth;

[AllowAnonymous]
public class AuthEntrarModel(IDataProtectionProvider dataProtectionProvider) : PageModel
{
    private readonly IDataProtector _protector =
        dataProtectionProvider.CreateProtector("SistemaRestaurante.Auth");

    public async Task<IActionResult> OnGetAsync(string payload, string returnUrl = "/")
    {
        try
        {
            var json = _protector.Unprotect(payload);
            var dto = JsonSerializer.Deserialize<LogarUsuario>(json)!;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString()),
                new Claim(ClaimTypes.Name, dto.Nome),
                new Claim("SetorId", dto.SetorId.ToString()),
                new Claim("SetorNome", dto.SetorNome),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return LocalRedirect(returnUrl);
        }
        catch
        {
            return Redirect("/login");
        }
    }
}
