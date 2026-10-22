using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fisgada.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriaEspeciesEPescarias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomePopular = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NomeCientifico = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pescarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EspecieId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Isca = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    TamanhoCm = table.Column<decimal>(type: "decimal(6,1)", precision: 6, scale: 1, nullable: true),
                    PesoKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FoiSolto = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pescarias", x => x.Id);
                    table.CheckConstraint("CK_Pescaria_Nota", "[Nota] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Pescarias_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Especies",
                columns: new[] { "Id", "FotoUrl", "NomeCientifico", "NomePopular" },
                values: new object[,]
                {
                    { 1, null, "Centropomus undecimalis", "Robalo-flecha" },
                    { 2, null, "Centropomus parallelus", "Robalo-peva" },
                    { 3, null, "Mugil liza", "Tainha" },
                    { 4, null, "Mugil curema", "Parati" },
                    { 5, null, "Micropogonias furnieri", "Corvina" },
                    { 6, null, "Cynoscion acoupa", "Pescada-amarela" },
                    { 7, null, "Cynoscion leiarchus", "Pescada-branca" },
                    { 8, null, "Menticirrhus americanus", "Betara" },
                    { 9, null, "Genidens barbus", "Bagre-branco" },
                    { 10, null, "Caranx hippos", "Xaréu" },
                    { 11, null, "Caranx crysos", "Carapau" },
                    { 12, null, "Scomberomorus brasiliensis", "Sororoca" },
                    { 13, null, "Trichiurus lepturus", "Espada" },
                    { 14, null, "Pomatomus saltatrix", "Anchova" },
                    { 15, null, "Trachinotus carolinus", "Pampo" },
                    { 16, null, "Eugerres brasilianus", "Caratinga" },
                    { 17, null, "Diapterus rhombeus", "Carapeba" },
                    { 18, null, "Paralichthys orbignyanus", "Linguado" },
                    { 19, null, "Epinephelus marginatus", "Garoupa" },
                    { 20, null, "Sphoeroides testudineus", "Baiacu" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Especies_NomePopular",
                table: "Especies",
                column: "NomePopular",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pescarias_EspecieId",
                table: "Pescarias",
                column: "EspecieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pescarias");

            migrationBuilder.DropTable(
                name: "Especies");
        }
    }
}
