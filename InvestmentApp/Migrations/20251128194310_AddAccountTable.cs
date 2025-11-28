using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    ChequingAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_ChequingAccounts_ChequingAccountId",
                        column: x => x.ChequingAccountId,
                        principalTable: "ChequingAccounts",
                        principalColumn: "ChequingAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ChequingAccountId",
                table: "Accounts",
                column: "ChequingAccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
