using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualWallet.DATA.Migrations
{
    public partial class AddedFeeAmountToWalletTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FeeAmount",
                table: "WalletTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeAmount",
                table: "WalletTransactions");
        }
    }
}
