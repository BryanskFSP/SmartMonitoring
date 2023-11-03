using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMonitoring.Server.Migrations
{
    /// <inheritdoc />
    public partial class LogUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DataBaseID",
                table: "Logs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_DataBaseID",
                table: "Logs",
                column: "DataBaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_DataBases_DataBaseID",
                table: "Logs",
                column: "DataBaseID",
                principalTable: "DataBases",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_DataBases_DataBaseID",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_DataBaseID",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "DataBaseID",
                table: "Logs");
        }
    }
}
