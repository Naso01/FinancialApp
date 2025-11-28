using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingsAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ChequingAccounts_ChequingAccountId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ChequingAccountId",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "ChequingAccountId",
                table: "Accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SavingsAccountId",
                table: "Accounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SavingsAccounts",
                columns: table => new
                {
                    SavingsAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsAccounts", x => x.SavingsAccountId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ChequingAccountId",
                table: "Accounts",
                column: "ChequingAccountId",
                unique: true,
                filter: "[ChequingAccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_SavingsAccountId",
                table: "Accounts",
                column: "SavingsAccountId",
                unique: true,
                filter: "[SavingsAccountId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ChequingAccounts_ChequingAccountId",
                table: "Accounts",
                column: "ChequingAccountId",
                principalTable: "ChequingAccounts",
                principalColumn: "ChequingAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_SavingsAccounts_SavingsAccountId",
                table: "Accounts",
                column: "SavingsAccountId",
                principalTable: "SavingsAccounts",
                principalColumn: "SavingsAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ChequingAccounts_ChequingAccountId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_SavingsAccounts_SavingsAccountId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "SavingsAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ChequingAccountId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_SavingsAccountId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "SavingsAccountId",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "ChequingAccountId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ChequingAccountId",
                table: "Accounts",
                column: "ChequingAccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ChequingAccounts_ChequingAccountId",
                table: "Accounts",
                column: "ChequingAccountId",
                principalTable: "ChequingAccounts",
                principalColumn: "ChequingAccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
