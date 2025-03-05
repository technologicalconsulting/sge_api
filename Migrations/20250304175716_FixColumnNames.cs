using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace sge_api.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Empresas_EmpresaId",
                table: "Empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Empleados_EmpleadoId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "VerificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empleados",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "NumeroIdentificacion",
                table: "Empleados");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Empleados",
                newName: "empleado");

            migrationBuilder.RenameColumn(
                name: "Usuario",
                table: "users",
                newName: "usuario");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "users",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "EmpleadoId",
                table: "users",
                newName: "empleadoid");

            migrationBuilder.RenameColumn(
                name: "Bloqueado",
                table: "users",
                newName: "bloqueado");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "IntentosFallidos",
                table: "users",
                newName: "intentos_fallidos");

            migrationBuilder.RenameColumn(
                name: "FechaUltimoLogin",
                table: "users",
                newName: "fecha_ultimo_login");

            migrationBuilder.RenameIndex(
                name: "IX_Users_EmpleadoId",
                table: "users",
                newName: "IX_users_empleadoid");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Empresas",
                newName: "telefono");

            migrationBuilder.RenameColumn(
                name: "Sector",
                table: "Empresas",
                newName: "sector");

            migrationBuilder.RenameColumn(
                name: "RUC",
                table: "Empresas",
                newName: "ruc");

            migrationBuilder.RenameColumn(
                name: "Pais",
                table: "Empresas",
                newName: "pais");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Empresas",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Empresas",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Empresas",
                newName: "direccion");

            migrationBuilder.RenameColumn(
                name: "Ciudad",
                table: "Empresas",
                newName: "ciudad");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Empresas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoEmpresa",
                table: "Empresas",
                newName: "tipo_empresa");

            migrationBuilder.RenameColumn(
                name: "SitioWeb",
                table: "Empresas",
                newName: "sitio_web");

            migrationBuilder.RenameColumn(
                name: "RazonSocial",
                table: "Empresas",
                newName: "razon_social");

            migrationBuilder.RenameColumn(
                name: "NombreComercial",
                table: "Empresas",
                newName: "nombre_comercial");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "Empresas",
                newName: "logo_url");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "Empresas",
                newName: "fecha_registro");

            migrationBuilder.RenameColumn(
                name: "FechaFundacion",
                table: "Empresas",
                newName: "fecha_fundacion");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "empleado",
                newName: "telefono");

            migrationBuilder.RenameColumn(
                name: "Genero",
                table: "empleado",
                newName: "genero");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "empleado",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "empleado",
                newName: "direccion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "empleado",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoDocumento",
                table: "empleado",
                newName: "tipo_documento");

            migrationBuilder.RenameColumn(
                name: "SegundoNombre",
                table: "empleado",
                newName: "segundo_nombre");

            migrationBuilder.RenameColumn(
                name: "PrimerNombre",
                table: "empleado",
                newName: "primer_nombre");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "empleado",
                newName: "fecha_registro");

            migrationBuilder.RenameColumn(
                name: "FechaNacimiento",
                table: "empleado",
                newName: "fecha_nacimiento");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "empleado",
                newName: "empresa_Id");

            migrationBuilder.RenameColumn(
                name: "EmailPersonal",
                table: "empleado",
                newName: "numero_identificacion");

            migrationBuilder.RenameColumn(
                name: "EmailCorporativo",
                table: "empleado",
                newName: "email_corporativo");

            migrationBuilder.RenameColumn(
                name: "ApellidoPaterno",
                table: "empleado",
                newName: "email_personal");

            migrationBuilder.RenameColumn(
                name: "ApellidoMaterno",
                table: "empleado",
                newName: "apellido_materno");

            migrationBuilder.RenameIndex(
                name: "IX_Empleados_EmpresaId",
                table: "empleado",
                newName: "IX_empleado_empresa_Id");

            migrationBuilder.AlterColumn<string>(
                name: "usuario",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "empleado_id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_registro",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "numero_identificacion",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "apellido_paterno",
                table: "empleado",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_empleado",
                table: "empleado",
                column: "id");

            migrationBuilder.CreateTable(
                name: "CodigosVerificacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    usuarioid = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usado = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "IX_users_usuario",
                table: "users",
                column: "usuario",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_empleado_Empresas_empresa_Id",
                table: "empleado");

            migrationBuilder.DropForeignKey(
                name: "FK_users_empleado_empleadoid",
                table: "users");

            migrationBuilder.DropTable(
                name: "CodigosVerificacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_usuario",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_empleado",
                table: "empleado");

            migrationBuilder.DropIndex(
                name: "IX_empleado_numero_identificacion",
                table: "empleado");

            migrationBuilder.DropColumn(
                name: "empleado_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "fecha_registro",
                table: "users");

            migrationBuilder.DropColumn(
                name: "numero_identificacion",
                table: "users");

            migrationBuilder.DropColumn(
                name: "apellido_paterno",
                table: "empleado");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "empleado",
                newName: "Empleados");

            migrationBuilder.RenameColumn(
                name: "usuario",
                table: "Users",
                newName: "Usuario");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Users",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "empleadoid",
                table: "Users",
                newName: "EmpleadoId");

            migrationBuilder.RenameColumn(
                name: "bloqueado",
                table: "Users",
                newName: "Bloqueado");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "intentos_fallidos",
                table: "Users",
                newName: "IntentosFallidos");

            migrationBuilder.RenameColumn(
                name: "fecha_ultimo_login",
                table: "Users",
                newName: "FechaUltimoLogin");

            migrationBuilder.RenameIndex(
                name: "IX_users_empleadoid",
                table: "Users",
                newName: "IX_Users_EmpleadoId");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "Empresas",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "sector",
                table: "Empresas",
                newName: "Sector");

            migrationBuilder.RenameColumn(
                name: "ruc",
                table: "Empresas",
                newName: "RUC");

            migrationBuilder.RenameColumn(
                name: "pais",
                table: "Empresas",
                newName: "Pais");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Empresas",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Empresas",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "direccion",
                table: "Empresas",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "ciudad",
                table: "Empresas",
                newName: "Ciudad");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Empresas",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_empresa",
                table: "Empresas",
                newName: "TipoEmpresa");

            migrationBuilder.RenameColumn(
                name: "sitio_web",
                table: "Empresas",
                newName: "SitioWeb");

            migrationBuilder.RenameColumn(
                name: "razon_social",
                table: "Empresas",
                newName: "RazonSocial");

            migrationBuilder.RenameColumn(
                name: "nombre_comercial",
                table: "Empresas",
                newName: "NombreComercial");

            migrationBuilder.RenameColumn(
                name: "logo_url",
                table: "Empresas",
                newName: "LogoUrl");

            migrationBuilder.RenameColumn(
                name: "fecha_registro",
                table: "Empresas",
                newName: "FechaRegistro");

            migrationBuilder.RenameColumn(
                name: "fecha_fundacion",
                table: "Empresas",
                newName: "FechaFundacion");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "Empleados",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "genero",
                table: "Empleados",
                newName: "Genero");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Empleados",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "direccion",
                table: "Empleados",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Empleados",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_documento",
                table: "Empleados",
                newName: "TipoDocumento");

            migrationBuilder.RenameColumn(
                name: "segundo_nombre",
                table: "Empleados",
                newName: "SegundoNombre");

            migrationBuilder.RenameColumn(
                name: "primer_nombre",
                table: "Empleados",
                newName: "PrimerNombre");

            migrationBuilder.RenameColumn(
                name: "numero_identificacion",
                table: "Empleados",
                newName: "EmailPersonal");

            migrationBuilder.RenameColumn(
                name: "fecha_registro",
                table: "Empleados",
                newName: "FechaRegistro");

            migrationBuilder.RenameColumn(
                name: "fecha_nacimiento",
                table: "Empleados",
                newName: "FechaNacimiento");

            migrationBuilder.RenameColumn(
                name: "empresa_Id",
                table: "Empleados",
                newName: "EmpresaId");

            migrationBuilder.RenameColumn(
                name: "email_personal",
                table: "Empleados",
                newName: "ApellidoPaterno");

            migrationBuilder.RenameColumn(
                name: "email_corporativo",
                table: "Empleados",
                newName: "EmailCorporativo");

            migrationBuilder.RenameColumn(
                name: "apellido_materno",
                table: "Empleados",
                newName: "ApellidoMaterno");

            migrationBuilder.RenameIndex(
                name: "IX_empleado_empresa_Id",
                table: "Empleados",
                newName: "IX_Empleados_EmpresaId");

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "NumeroIdentificacion",
                table: "Empleados",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empleados",
                table: "Empleados",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Usado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_UserId",
                table: "VerificationCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Empresas_EmpresaId",
                table: "Empleados",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Empleados_EmpleadoId",
                table: "Users",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
