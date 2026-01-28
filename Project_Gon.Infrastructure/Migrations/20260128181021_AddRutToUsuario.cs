using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Gon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRutToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EmpresaId_Email",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "Rut",
                table: "Usuarios",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId_Email",
                table: "Usuarios",
                columns: new[] { "EmpresaId", "Email" },
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId_Rut",
                table: "Usuarios",
                columns: new[] { "EmpresaId", "Rut" },
                unique: true,
                filter: "\"Rut\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EmpresaId_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EmpresaId_Rut",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Rut",
                table: "Usuarios");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId_Email",
                table: "Usuarios",
                columns: new[] { "EmpresaId", "Email" },
                unique: true);
        }
    }
}
