using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaRestaurante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCliente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroMesa = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    SetorPreparoId = table.Column<int>(type: "int", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Setores_SetorPreparoId",
                        column: x => x.SetorPreparoId,
                        principalTable: "Setores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HashSenha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SetorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Setores_SetorId",
                        column: x => x.SetorId,
                        principalTable: "Setores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemPedido",
                columns: table => new
                {
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPedido", x => new { x.PedidoId, x.ProdutoId });
                    table.ForeignKey(
                        name: "FK_ItemPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPedido_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Setores",
                columns: new[] { "Id", "Nome", "Tipo" },
                values: new object[,]
                {
                    { 1, "Salão", 1 },
                    { 2, "Copa", 2 },
                    { 3, "Cozinha", 2 }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Disponivel", "Nome", "Preco", "SetorPreparoId" },
                values: new object[,]
                {
                    { 1, true, "Frango Grelhado", 25.899999999999999, 3 },
                    { 2, true, "Filé ao Molho", 45.0, 3 },
                    { 3, true, "Suco de Laranja", 8.0, 2 },
                    { 4, true, "Refrigerante", 6.0, 2 },
                    { 5, false, "Prato do Dia", 22.0, 3 }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "HashSenha", "Nome", "SetorId" },
                values: new object[,]
                {
                    { 1, "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm", "Garçon", 1 },
                    { 2, "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm", "Copa", 2 },
                    { 3, "$2a$12$VEYlve5PMZs6B.0WyS7O4uhwcrRexc62CNZmTp2OEthHI7ZSzH4lm", "Cozinha", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_ProdutoId",
                table: "ItemPedido",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SetorPreparoId",
                table: "Produtos",
                column: "SetorPreparoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_SetorId",
                table: "Usuarios",
                column: "SetorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemPedido");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Setores");
        }
    }
}
