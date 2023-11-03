using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMonitoring.Server.Migrations
{
    /// <inheritdoc />
    public partial class LogTwoUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FixStatus",
                table: "Logs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Logs",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixStatus",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Logs");
        }
    }
}
