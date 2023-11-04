using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMonitoring.Server.Migrations
{
    /// <inheritdoc />
    public partial class WikiAddMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wikies",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wikies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WikiSolutions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    WikiID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SqlScript = table.Column<string>(type: "text", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiSolutions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WikiSolutions_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WikiSolutions_Wikies_WikiID",
                        column: x => x.WikiID,
                        principalTable: "Wikies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wikies_Type",
                table: "Wikies",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikiSolutions_OrganizationID",
                table: "WikiSolutions",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_WikiSolutions_WikiID",
                table: "WikiSolutions",
                column: "WikiID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WikiSolutions");

            migrationBuilder.DropTable(
                name: "Wikies");
        }
    }
}
