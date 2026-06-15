using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Devsu.Cuentas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V1_CrearEsquemaCuentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cliente_ref",
                columns: table => new
                {
                    cliente_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente_ref", x => x.cliente_id);
                });

            migrationBuilder.CreateTable(
                name: "cuenta",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero_cuenta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tipo_cuenta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    saldo_inicial = table.Column<decimal>(type: "numeric(19,2)", precision: 19, scale: 2, nullable: false),
                    saldo_disponible = table.Column<decimal>(type: "numeric(19,2)", precision: 19, scale: 2, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false),
                    cliente_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuenta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movimiento",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    tipo_movimiento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(19,2)", precision: 19, scale: 2, nullable: false),
                    saldo = table.Column<decimal>(type: "numeric(19,2)", precision: 19, scale: 2, nullable: false),
                    cuenta_id = table.Column<long>(type: "bigint", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimiento", x => x.id);
                    table.ForeignKey(
                        name: "fk_movimiento_cuenta",
                        column: x => x.cuenta_id,
                        principalTable: "cuenta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "uk_cuenta_numero",
                table: "cuenta",
                column: "numero_cuenta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_movimiento_cuenta",
                table: "movimiento",
                column: "cuenta_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cliente_ref");

            migrationBuilder.DropTable(
                name: "movimiento");

            migrationBuilder.DropTable(
                name: "cuenta");
        }
    }
}
