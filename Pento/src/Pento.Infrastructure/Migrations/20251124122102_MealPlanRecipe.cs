using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MealPlanRecipe : Migration
{
    private static readonly string[] columns = new[] { "meal_plan_id", "recipe_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_recipe_recipe_id",
            table: "meal_plans");

        migrationBuilder.DropIndex(
            name: "ix_meal_plans_recipe_id",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "food_item_id",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "recipe_id",
            table: "meal_plans");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            table: "meal_plans",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(200)",
            oldMaxLength: 200);

        migrationBuilder.CreateTable(
            name: "meal_plan_recipes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plan_recipes", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plan_recipes_meal_plans_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_recipes_meal_plan_id_recipe_id",
            table: "meal_plan_recipes",
            columns: columns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "meal_plan_recipes");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            table: "meal_plans",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<Guid>(
            name: "food_item_id",
            table: "meal_plans",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "recipe_id",
            table: "meal_plans",
            type: "uuid",
            nullable: true);

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
            onDelete: ReferentialAction.SetNull);
    }
}
