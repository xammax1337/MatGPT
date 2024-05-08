using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatGPT.Migrations
{
    public partial class KitchenSupplyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitchenSupply_Users_UserId",
                table: "KitchenSupply");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KitchenSupply",
                table: "KitchenSupply");

            migrationBuilder.RenameTable(
                name: "KitchenSupply",
                newName: "KitchenSupplies");

            migrationBuilder.RenameIndex(
                name: "IX_KitchenSupply_UserId",
                table: "KitchenSupplies",
                newName: "IX_KitchenSupplies_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KitchenSupplies",
                table: "KitchenSupplies",
                column: "KitchenSupplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_KitchenSupplies_Users_UserId",
                table: "KitchenSupplies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitchenSupplies_Users_UserId",
                table: "KitchenSupplies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KitchenSupplies",
                table: "KitchenSupplies");

            migrationBuilder.RenameTable(
                name: "KitchenSupplies",
                newName: "KitchenSupply");

            migrationBuilder.RenameIndex(
                name: "IX_KitchenSupplies_UserId",
                table: "KitchenSupply",
                newName: "IX_KitchenSupply_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KitchenSupply",
                table: "KitchenSupply",
                column: "KitchenSupplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_KitchenSupply_Users_UserId",
                table: "KitchenSupply",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
