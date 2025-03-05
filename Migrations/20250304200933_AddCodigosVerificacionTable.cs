using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace sge_api.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigosVerificacionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_empleado_Empresas_empresa_Id",
                table: "empleado");

            migrationBuilder.DropForeignKey(
                name: "FK_users_empleado_empleadoid",
                table: "users");

            migrationBuilder.DropTable(
                name: "CodigosVerificacion");

            migrationBuilder.DropIndex(
                name: "IX_users_empleadoid",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_usuario",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_empleado_empresa_Id",
                table: "empleado");

            migrationBuilder.DropIndex(
                name: "IX_empleado_numero_identificacion",
                table: "empleado");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empresas",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "empleadoid",
                table: "users");

            migrationBuilder.RenameTable(
                name: "Empresas",
                newName: "empresa");

            migrationBuilder.RenameColumn(
                name: "empresa_Id",
                table: "empleado",
                newName: "empresa_id");

            migrationBuilder.AlterColumn<string>(
                name: "pais",
                table: "empresa",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "empresa",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_empresa",
                table: "empresa",
                column: "id");

            migrationBuilder.CreateTable(
                name: "codigos_verificacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codigos_verificacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_codigos_verificacion_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_empleado_id",
                table: "users",
                column: "empleado_id");

            migrationBuilder.CreateIndex(
                name: "IX_codigos_verificacion_usuario_id",
                table: "codigos_verificacion",
                column: "usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_empleado_empleado_id",
                table: "users",
                column: "empleado_id",
                principalTable: "empleado",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_empleado_empleado_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "codigos_verificacion");

            migrationBuilder.DropIndex(
                name: "IX_users_empleado_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_empresa",
                table: "empresa");

            migrationBuilder.RenameTable(
                name: "empresa",
                newName: "Empresas");

            migrationBuilder.RenameColumn(
                name: "empresa_id",
                table: "empleado",
                newName: "empresa_Id");

            migrationBuilder.AddColumn<int>(
                name: "empleadoid",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "pais",
                table: "Empresas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "Empresas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empresas",
                table: "Empresas",
                column: "id");

            migrationBuilder.CreateTable(
                name: "CodigosVerificacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuarioid = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    usado = table.Column<bool>(type: "boolean", nullable: false),
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigosVerificacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_CodigosVerificacion_users_usuarioid",
                        column: x => x.usuarioid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_empleadoid",
                table: "users",
                column: "empleadoid");

            migrationBuilder.CreateIndex(
                name: "IX_users_usuario",
                table: "users",
                column: "usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleado_empresa_Id",
                table: "empleado",
                column: "empresa_Id");

            migrationBuilder.CreateIndex(
                name: "IX_empleado_numero_identificacion",
                table: "empleado",
                column: "numero_identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodigosVerificacion_usuarioid",
                table: "CodigosVerificacion",
                column: "usuarioid");

            migrationBuilder.AddForeignKey(
                name: "FK_empleado_Empresas_empresa_Id",
                table: "empleado",
                column: "empresa_Id",
                principalTable: "Empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_empleado_empleadoid",
                table: "users",
                column: "empleadoid",
                principalTable: "empleado",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
