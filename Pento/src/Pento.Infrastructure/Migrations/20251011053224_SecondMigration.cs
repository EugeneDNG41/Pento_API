using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class SecondMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "food_references",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                food_group = table.Column<string>(type: "text", nullable: false),
                barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                typical_shelf_life_days = table.Column<int>(type: "integer", nullable: false),
                open_food_facts_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                usda_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_references", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "household",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_household", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "recipes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                recipe_prep_time_minutes = table.Column<int>(type: "integer", nullable: true),
                recipe_cook_time_minutes = table.Column<int>(type: "integer", nullable: true),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                servings = table.Column<int>(type: "integer", nullable: true),
                difficulty_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                calories_per_serving = table.Column<int>(type: "integer", nullable: true),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                prep_time_minutes = table.Column<int>(type: "integer", nullable: false),
                cook_time_minutes = table.Column<int>(type: "integer", nullable: false),
                is_public = table.Column<bool>(type: "boolean", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipes", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "meal_plans",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                start_date = table.Column<DateOnly>(type: "date", nullable: false),
                end_date = table.Column<DateOnly>(type: "date", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plans", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plans_household_household_id",
                    column: x => x.household_id,
                    principalTable: "household",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "meal_plan_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                servings = table.Column<int>(type: "integer", nullable: false),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                schedule = table.Column<string>(type: "TEXT", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plan_items", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plan_items_meal_plans_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_meal_plan_items_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_items_meal_plan_id",
            table: "meal_plan_items",
            column: "meal_plan_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_items_recipe_id",
            table: "meal_plan_items",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_household_id",
            table: "meal_plans",
            column: "household_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "food_references");

        migrationBuilder.DropTable(
            name: "meal_plan_items");

        migrationBuilder.DropTable(
            name: "meal_plans");

        migrationBuilder.DropTable(
            name: "recipes");

        migrationBuilder.DropTable(
            name: "household");
    }
}
