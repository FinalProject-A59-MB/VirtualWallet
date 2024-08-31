using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualWallet.DATA.Migrations
{
    public partial class WalletTransactionFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "SortBy",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WalletTransactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SortBy",
                table: "WalletTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SortOrder",
                table: "WalletTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
