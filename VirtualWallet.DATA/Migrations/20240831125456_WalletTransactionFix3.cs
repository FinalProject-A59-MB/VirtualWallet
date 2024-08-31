using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualWallet.DATA.Migrations
{
    public partial class WalletTransactionFix3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "WalletTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "WalletTransactions");
        }
    }
}
