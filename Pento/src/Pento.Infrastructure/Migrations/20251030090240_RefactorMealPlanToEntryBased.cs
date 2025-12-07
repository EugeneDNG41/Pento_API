using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RefactorMealPlanToEntryBased : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "meal_plan_items");

        migrationBuilder.DropColumn(
            name: "end_date",
            table: "meal_plans");

        migrationBuilder.RenameColumn(
            name: "start_date",
            table: "meal_plans",
            newName: "scheduled_date");

        migrationBuilder.AddColumn<int>(
            name: "meal_type",
            table: "meal_plans",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "notes",
            table: "meal_plans",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "recipe_id",
            table: "meal_plans",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<int>(
            name: "servings",
            table: "meal_plans",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_recipe_id",
            table: "meal_plans",
            column: "recipe_id");

        migrationBuilder.AddForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans");

        migrationBuilder.DropIndex(
            name: "ix_meal_plans_recipe_id",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "meal_type",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "notes",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "recipe_id",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "servings",
            table: "meal_plans");

        migrationBuilder.RenameColumn(
            name: "scheduled_date",
            table: "meal_plans",
            newName: "start_date");

        migrationBuilder.AddColumn<DateOnly>(
            name: "end_date",
            table: "meal_plans",
            type: "date",
            nullable: false,
            defaultValue: new DateOnly(1, 1, 1));

        migrationBuilder.CreateTable(
            name: "meal_plan_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                schedule = table.Column<string>(type: "TEXT", nullable: false),
                servings = table.Column<int>(type: "integer", nullable: false),
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
    }
}
