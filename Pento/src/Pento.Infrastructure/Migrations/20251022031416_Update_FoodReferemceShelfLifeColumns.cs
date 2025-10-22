using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Update_FoodReferemceShelfLifeColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "typical_shelf_life_days",
            table: "food_references",
            newName: "typical_shelf_life_days_pantry");

        migrationBuilder.AddColumn<int>(
            name: "typical_shelf_life_days_freezer",
            table: "food_references",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "typical_shelf_life_days_fridge",
            table: "food_references",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "typical_shelf_life_days_freezer",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "typical_shelf_life_days_fridge",
            table: "food_references");

        migrationBuilder.RenameColumn(
            name: "typical_shelf_life_days_pantry",
            table: "food_references",
            newName: "typical_shelf_life_days");
    }
}
