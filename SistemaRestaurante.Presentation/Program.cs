using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SistemaRestaurante.Presentation.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ISenhaHasher, BCryptSenhaHasher>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<IAutenticacaoServico>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<ISetorRepositorio, SetorRepositorioEmMemoria>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioEmMemoria>();
builder.Services.AddScoped<UsuarioLogaUseCase>();


builder.Services.AddAuthentication();
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
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
