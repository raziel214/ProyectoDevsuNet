using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Devsu.Clientes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V1_CrearTablaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    genero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    edad = table.Column<int>(type: "integer", nullable: true),
                    identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cliente_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contrasena = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "uk_cliente_cliente_id",
                table: "cliente",
                column: "cliente_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_cliente_identificacion",
                table: "cliente",
                column: "identificacion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cliente");
        }
    }
}
