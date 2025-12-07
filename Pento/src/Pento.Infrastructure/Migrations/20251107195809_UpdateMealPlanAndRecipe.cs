using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateMealPlanAndRecipe : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans");

        migrationBuilder.AlterColumn<int>(
            name: "recipe_prep_time_minutes",
            table: "recipes",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "recipe_cook_time_minutes",
            table: "recipes",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            table: "recipes",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(2000)",
            oldMaxLength: 2000,
            oldNullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "recipe_id",
            table: "meal_plans",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.AddColumn<Guid>(
            name: "food_item_id",
            table: "meal_plans",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "food_item",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                version = table.Column<int>(type: "integer", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                image_url = table.Column<string>(type: "text", nullable: true),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                expiration_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                notes = table.Column<string>(type: "text", nullable: true),
                source_item_id = table.Column<Guid>(type: "uuid", nullable: true)
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

        migrationBuilder.AddForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_food_item_food_item_id",
            table: "meal_plans");

        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans");

        migrationBuilder.DropTable(
            name: "food_item");

        migrationBuilder.DropIndex(
            name: "ix_meal_plans_food_item_id",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "food_item_id",
            table: "meal_plans");

        migrationBuilder.AlterColumn<int>(
            name: "recipe_prep_time_minutes",
            table: "recipes",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<int>(
            name: "recipe_cook_time_minutes",
            table: "recipes",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "description",
            table: "recipes",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(2000)",
            oldMaxLength: 2000);

        migrationBuilder.AlterColumn<Guid>(
            name: "recipe_id",
            table: "meal_plans",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
