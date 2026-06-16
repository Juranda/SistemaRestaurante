using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using SistemaRestaurante.Application.InterfacesDeServicos;
using SistemaRestaurante.Application.UseCases;
using SistemaRestaurante.Domain.Repositorios;
using SistemaRestaurante.Infrastructure.Autenticacao;
using SistemaRestaurante.Infrastructure.Repositorio;
using SistemaRestaurante.Infrastructure.Servicos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

builder.Services.AddSingleton<ISenhaHasher, BCryptSenhaHasher>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<IAutenticacaoServico>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddSingleton<ISetorRepositorio, SetorRepositorioEmMemoria>();
builder.Services.AddSingleton<IProdutoRepositorio, ProdutoRepositorioEmMemoria>();
builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEmMemoria>();
builder.Services.AddSingleton<IUsuarioRepositorio, UsuarioRepositorioEmMemoria>();
builder.Services.AddSingleton<IItemPedidoRepositorio, ItemPedidoRepositorioEmMemoria>();

builder.Services.AddScoped<UsuarioAlteraStatusDoItemDoPedido>();
builder.Services.AddScoped<UsuarioRegistraPedidoUseCase>();
builder.Services.AddScoped<UsuarioLogaUseCase>();
builder.Services.AddScoped<HistoricoPedidosUseCase>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Setor", policy =>
    {
        policy.RequireClaim("SetorId");
        policy.RequireClaim("SetorNome");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();