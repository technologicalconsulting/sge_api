using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace sge_api.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorialEventosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialEventosUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    EmpleadoId = table.Column<int>(type: "integer", nullable: true),
                    TipoEvento = table.Column<string>(type: "text", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Exito = table.Column<bool>(type: "boolean", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Navegador = table.Column<string>(type: "text", nullable: false),
                    Razon = table.Column<string>(type: "text", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEventosUsuario", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_empleado_empresa_id",
                table: "empleado",
                column: "empresa_id");

            migrationBuilder.AddForeignKey(
                name: "FK_empleado_empresa_empresa_id",
                table: "empleado",
                column: "empresa_id",
                principalTable: "empresa",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_empleado_empresa_empresa_id",
                table: "empleado");

            migrationBuilder.DropTable(
                name: "HistorialEventosUsuario");

            migrationBuilder.DropIndex(
                name: "IX_empleado_empresa_id",
                table: "empleado");
        }
    }
}
