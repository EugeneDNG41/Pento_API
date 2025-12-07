using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveFoodItemTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_food_item_food_item_id",
            table: "meal_plans");

        migrationBuilder.DropTable(
            name: "food_item");

        migrationBuilder.DropIndex(
            name: "ix_meal_plans_food_item_id",
            table: "meal_plans");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "food_item",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                expiration_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                image_url = table.Column<string>(type: "text", nullable: true),
                name = table.Column<string>(type: "text", nullable: false),
                notes = table.Column<string>(type: "text", nullable: true),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                source_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_item", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_food_item_id",
            table: "meal_plans",
            column: "food_item_id");

        migrationBuilder.AddForeignKey(
            name: "fk_meal_plans_food_item_food_item_id",
            table: "meal_plans",
            column: "food_item_id",
            principalTable: "food_item",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }
}
