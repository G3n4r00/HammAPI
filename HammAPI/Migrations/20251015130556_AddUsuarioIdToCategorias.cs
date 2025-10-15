using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HammAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIdToCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "id_usuario",
                schema: "public",
                table: "categorias",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_categorias_id_usuario",
                schema: "public",
                table: "categorias",
                column: "id_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_categorias_usuarios_id_usuario",
                schema: "public",
                table: "categorias",
                column: "id_usuario",
                principalSchema: "public",
                principalTable: "usuarios",
                principalColumn: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categorias_usuarios_id_usuario",
                schema: "public",
                table: "categorias");

            migrationBuilder.DropIndex(
                name: "IX_categorias_id_usuario",
                schema: "public",
                table: "categorias");

            migrationBuilder.DropColumn(
                name: "id_usuario",
                schema: "public",
                table: "categorias");
        }
    }
}
