using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualWallet.DATA.Migrations
{
    public partial class WalletTransactionFix4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "WalletTransactions",
                newName: "WithdrownAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "DepositedAmount",
                table: "WalletTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositedAmount",
                table: "WalletTransactions");

            migrationBuilder.RenameColumn(
                name: "WithdrownAmount",
                table: "WalletTransactions",
                newName: "Amount");
        }
    }
}
