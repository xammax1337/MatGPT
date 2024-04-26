using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatGPT.Migrations
{
    public partial class InitFixedContex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pantry_Users_UserId",
                table: "Pantry");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryFoodItem_FoodItems_FoodItemId",
                table: "PantryFoodItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryFoodItem_Pantry_PantryId",
                table: "PantryFoodItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PantryFoodItem",
                table: "PantryFoodItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pantry",
                table: "Pantry");

            migrationBuilder.RenameTable(
                name: "PantryFoodItem",
                newName: "PantryFoodItems");

            migrationBuilder.RenameTable(
                name: "Pantry",
                newName: "Pantries");

            migrationBuilder.RenameIndex(
                name: "IX_PantryFoodItem_PantryId",
                table: "PantryFoodItems",
                newName: "IX_PantryFoodItems_PantryId");

            migrationBuilder.RenameIndex(
                name: "IX_PantryFoodItem_FoodItemId",
                table: "PantryFoodItems",
                newName: "IX_PantryFoodItems_FoodItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Pantry_UserId",
                table: "Pantries",
                newName: "IX_Pantries_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PantryFoodItems",
                table: "PantryFoodItems",
                column: "PantryFoodItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pantries",
                table: "Pantries",
                column: "PantryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pantries_Users_UserId",
                table: "Pantries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryFoodItems_FoodItems_FoodItemId",
                table: "PantryFoodItems",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "FoodItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryFoodItems_Pantries_PantryId",
                table: "PantryFoodItems",
                column: "PantryId",
                principalTable: "Pantries",
                principalColumn: "PantryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pantries_Users_UserId",
                table: "Pantries");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryFoodItems_FoodItems_FoodItemId",
                table: "PantryFoodItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryFoodItems_Pantries_PantryId",
                table: "PantryFoodItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PantryFoodItems",
                table: "PantryFoodItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pantries",
                table: "Pantries");

            migrationBuilder.RenameTable(
                name: "PantryFoodItems",
                newName: "PantryFoodItem");

            migrationBuilder.RenameTable(
                name: "Pantries",
                newName: "Pantry");

            migrationBuilder.RenameIndex(
                name: "IX_PantryFoodItems_PantryId",
                table: "PantryFoodItem",
                newName: "IX_PantryFoodItem_PantryId");

            migrationBuilder.RenameIndex(
                name: "IX_PantryFoodItems_FoodItemId",
                table: "PantryFoodItem",
                newName: "IX_PantryFoodItem_FoodItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Pantries_UserId",
                table: "Pantry",
                newName: "IX_Pantry_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PantryFoodItem",
                table: "PantryFoodItem",
                column: "PantryFoodItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pantry",
                table: "Pantry",
                column: "PantryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pantry_Users_UserId",
                table: "Pantry",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryFoodItem_FoodItems_FoodItemId",
                table: "PantryFoodItem",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "FoodItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryFoodItem_Pantry_PantryId",
                table: "PantryFoodItem",
                column: "PantryId",
                principalTable: "Pantry",
                principalColumn: "PantryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
