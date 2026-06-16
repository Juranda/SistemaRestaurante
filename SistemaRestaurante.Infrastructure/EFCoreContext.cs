using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Domain.Entidades;

namespace SistemaRestaurante.Infrastructure;

public class EFCoreContext(DbContextOptions<EFCoreContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelarProdutos(modelBuilder);
        ModelarPedidos(modelBuilder);
        ModelarSetor(modelBuilder);
        ModelarUsuario(modelBuilder);
        Seed(modelBuilder);
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Setor> Setores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }


    private static void ModelarProdutos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Produto>()
            .HasOne<Setor>()
            .WithMany()
            .HasForeignKey(x => x.SetorPreparoId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Produto>()
            .HasMany<ItemPedido>()
            .WithOne()
            .HasForeignKey(x => x.ProdutoId);

        modelBuilder.Entity<Produto>()
            .Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(Produto.MAX_CARACTERES_NOME);
    }

    private static void ModelarPedidos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Pedido>()
            .Ignore(p => p.Status)
            .Ignore(p => p.Pronto)
            .Ignore(p => p.Quantidade);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.ItemsPedido)
            .WithOne()
            .HasForeignKey(x => x.PedidoId);

        modelBuilder.Entity<Pedido>()
            .Navigation(p => p.ItemsPedido)
            .HasField("itemsPedido")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<ItemPedido>()
            .HasKey(x => new { x.PedidoId, x.ProdutoId });

        modelBuilder.Entity<Pedido>()
            .Property(x => x.NomeCliente)
            .IsRequired()
            .HasMaxLength(Pedido.MAX_CARACTERES_NOME_CLIENTE);

        modelBuilder.Entity<Pedido>()
            .Property(x => x.NumeroMesa)
            .IsRequired();
    }


    private static void ModelarSetor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Setor>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Setor>()
            .Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(Setor.MAX_CARACTERES_NOME);
    }

    private static void ModelarUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Usuario>()
            .Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(Usuario.MAX_CARACTERES_NOME);

        modelBuilder.Entity<Usuario>()
            .Property(x => x.HashSenha)
            .IsRequired()
            .HasMaxLength(Usuario.MAX_CARACTERES_SENHA);

        modelBuilder.Entity<Usuario>()
            .HasOne<Setor>()
            .WithMany()
            .HasForeignKey(u => u.SetorId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        const string hash = "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm";

        modelBuilder.Entity<Setor>().HasData(
            Setor.Criar(1, "Salão",   TipoSetor.CRIA_PEDIDO).Value!,
            Setor.Criar(2, "Copa",    TipoSetor.REALIZA_PEDIDO).Value!,
            Setor.Criar(3, "Cozinha", TipoSetor.REALIZA_PEDIDO).Value!
        );

        modelBuilder.Entity<Produto>().HasData(
            Produto.Criar(1, "Frango Grelhado", 25.90, setorPreparoId: 3, disponivel: true).Value!,
            Produto.Criar(2, "Filé ao Molho",   45.00, setorPreparoId: 3, disponivel: true).Value!,
            Produto.Criar(3, "Suco de Laranja",  8.00, setorPreparoId: 2, disponivel: true).Value!,
            Produto.Criar(4, "Refrigerante",     6.00, setorPreparoId: 2, disponivel: true).Value!,
            Produto.Criar(5, "Prato do Dia",    22.00, setorPreparoId: 3, disponivel: false).Value!
        );

        modelBuilder.Entity<Usuario>().HasData(
            Usuario.Criar(1, "Garçon",  hash, setorId: 1).Value!,
            Usuario.Criar(2, "Copa",    hash, setorId: 2).Value!,
            Usuario.Criar(3, "Cozinha", hash, setorId: 3).Value!
        );
    }
}