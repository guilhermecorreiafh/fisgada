using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisgada.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaDonoNaPescaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Pescarias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pescarias_UsuarioId",
                table: "Pescarias",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pescarias_Usuarios_UsuarioId",
                table: "Pescarias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pescarias_Usuarios_UsuarioId",
                table: "Pescarias");

            migrationBuilder.DropIndex(
                name: "IX_Pescarias_UsuarioId",
                table: "Pescarias");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Pescarias");
        }
    }
}
