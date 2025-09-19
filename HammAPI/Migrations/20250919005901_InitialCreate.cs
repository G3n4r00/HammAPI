using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HammAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "categorias",
                schema: "public",
                columns: table => new
                {
                    id_categoria = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    e_padrao = table.Column<bool>(type: "boolean", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                schema: "public",
                columns: table => new
                {
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    primeiro_nome = table.Column<string>(type: "text", nullable: false),
                    ultimo_nome = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    senha_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "metas",
                schema: "public",
                columns: table => new
                {
                    id_meta = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    valor_objetivo = table.Column<decimal>(type: "money", nullable: false),
                    valor_atual = table.Column<decimal>(type: "money", nullable: true),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dia_prazo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas", x => x.id_meta);
                    table.ForeignKey(
                        name: "FK_metas_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orcamento",
                schema: "public",
                columns: table => new
                {
                    id_orcamento = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    valor_limite = table.Column<decimal>(type: "money", nullable: false),
                    mes = table.Column<string>(type: "text", nullable: false),
                    ano = table.Column<string>(type: "text", nullable: false),
                    valor_utilizado = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orcamento", x => x.id_orcamento);
                    table.ForeignKey(
                        name: "FK_orcamento_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transacoes",
                schema: "public",
                columns: table => new
                {
                    id_transacao = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    id_categoria = table.Column<Guid>(type: "uuid", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    metodo_pagamento = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transacoes", x => x.id_transacao);
                    table.ForeignKey(
                        name: "FK_transacoes_categorias_id_categoria",
                        column: x => x.id_categoria,
                        principalSchema: "public",
                        principalTable: "categorias",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transacoes_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metas_id_usuario",
                schema: "public",
                table: "metas",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_orcamento_id_usuario",
                schema: "public",
                table: "orcamento",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_id_categoria",
                schema: "public",
                table: "transacoes",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_id_usuario",
                schema: "public",
                table: "transacoes",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "orcamento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "transacoes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "categorias",
                schema: "public");

            migrationBuilder.DropTable(
                name: "usuarios",
                schema: "public");
        }
    }
}
