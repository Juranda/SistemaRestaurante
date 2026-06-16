using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Infrastructure;
using SistemaRestaurante.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AdicionarServicos()
    .AdicionarRepositorios(builder.Configuration)
    .AdicionarCasosDeUso()
    .AdicionarAutenticacao();

var app = builder.Build();

await AplicarMigrationsAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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

static async Task AplicarMigrationsAsync(WebApplication app)
{
    const int maxTentativas = 10;
    for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EFCoreContext>();
            await db.Database.MigrateAsync();
            return;
        }
        catch (Exception ex) when (tentativa < maxTentativas)
        {
            Console.WriteLine($"[Migration] Tentativa {tentativa}/{maxTentativas} falhou: {ex.Message}. Aguardando 5s...");
            await Task.Delay(5000);
        }
    }
}
