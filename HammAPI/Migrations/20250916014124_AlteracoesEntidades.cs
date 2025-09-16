using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HammAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlteracoesEntidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "valor_limite",
                schema: "public",
                table: "orcamento",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "mes",
                schema: "public",
                table: "orcamento",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ano",
                schema: "public",
                table: "orcamento",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nome",
                schema: "public",
                table: "orcamento",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "valor_objetivo",
                schema: "public",
                table: "metas",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "public",
                table: "metas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "dia_prazo",
                schema: "public",
                table: "metas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inicio",
                schema: "public",
                table: "metas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                schema: "public",
                table: "metas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                schema: "public",
                table: "categorias",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nome",
                schema: "public",
                table: "orcamento");

            migrationBuilder.DropColumn(
                name: "data_inicio",
                schema: "public",
                table: "metas");

            migrationBuilder.DropColumn(
                name: "descricao",
                schema: "public",
                table: "metas");

            migrationBuilder.DropColumn(
                name: "descricao",
                schema: "public",
                table: "categorias");

            migrationBuilder.AlterColumn<decimal>(
                name: "valor_limite",
                schema: "public",
                table: "orcamento",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "mes",
                schema: "public",
                table: "orcamento",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ano",
                schema: "public",
                table: "orcamento",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "valor_objetivo",
                schema: "public",
                table: "metas",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "public",
                table: "metas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "dia_prazo",
                schema: "public",
                table: "metas",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
