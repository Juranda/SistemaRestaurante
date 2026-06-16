using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Application.InterfacesDeServicos;
using SistemaRestaurante.Application.UseCases;
using SistemaRestaurante.Domain.Repositorios;
using SistemaRestaurante.Infrastructure;
using SistemaRestaurante.Infrastructure.Autenticacao;
using SistemaRestaurante.Infrastructure.Repositorio;
using SistemaRestaurante.Infrastructure.Servicos;

namespace SistemaRestaurante.Presentation;

public static class SistemaRestauranteServiceExtensions
{
    public static IServiceCollection AdicionarServicos(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHttpContextAccessor();
        services.AddDataProtection();
        return services;
    }

    public static IServiceCollection AdicionarRepositorios(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EFCoreContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ISetorRepositorio, SetorRepositorioEFCore>();
        services.AddScoped<IProdutoRepositorio, ProdutoRepositorioEFCore>();
        services.AddScoped<IPedidoRepositorio, PedidoRepositorioEFCore>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioEFCore>();
        services.AddScoped<IItemPedidoRepositorio, ItemPedidoRepositorioEFCore>();
        return services;
    }

    public static IServiceCollection AdicionarCasosDeUso(this IServiceCollection services)
    {
        services.AddScoped<UsuarioAlteraStatusDoItemDoPedido>();
        services.AddScoped<UsuarioRegistraPedidoUseCase>();
        services.AddScoped<UsuarioLogaUseCase>();
        services.AddScoped<HistoricoPedidosUseCase>();
        return services;
    }

    public static IServiceCollection AdicionarAutenticacao(this IServiceCollection services)
    {
        services.AddSingleton<ISenhaHasher, BCryptSenhaHasher>();
        services.AddScoped<CustomAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
        services.AddScoped<IAutenticacaoServico>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("Setor", policy =>
            {
                policy.RequireClaim("SetorId");
                policy.RequireClaim("SetorNome");
            });
        });

        return services;
    }
}
