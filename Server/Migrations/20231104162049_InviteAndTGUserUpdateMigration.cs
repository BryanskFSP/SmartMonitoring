using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMonitoring.Server.Migrations
{
    /// <inheritdoc />
    public partial class InviteAndTGUserUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TelegramID",
                table: "TelegramUsers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminID",
                table: "TelegramUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UseCount",
                table: "Invites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                table: "Invites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_AdminID",
                table: "TelegramUsers",
                column: "AdminID");

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramUsers_Admins_AdminID",
                table: "TelegramUsers",
                column: "AdminID",
                principalTable: "Admins",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TelegramUsers_Admins_AdminID",
                table: "TelegramUsers");

            migrationBuilder.DropIndex(
                name: "IX_TelegramUsers_AdminID",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "AdminID",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "UseCount",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "UsedCount",
                table: "Invites");

            migrationBuilder.AlterColumn<int>(
                name: "TelegramID",
                table: "TelegramUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
