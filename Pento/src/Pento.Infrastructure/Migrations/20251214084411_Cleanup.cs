using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Cleanup : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_item_logs_units_unit_id",
            table: "food_item_logs");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_food_references_food_reference_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_units_unit_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_grocery_list_items_units_unit_id",
            table: "grocery_list_items");

        migrationBuilder.DropForeignKey(
            name: "fk_recipe_ingredients_units_unit_id",
            table: "recipe_ingredients");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_items_units_unit_id",
            table: "trade_items");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_session_items_units_unit_id",
            table: "trade_session_items");

        migrationBuilder.DropTable(
            name: "recipe_media");

        migrationBuilder.CreateIndex(
            name: "ix_user_milestones_user_id",
            table: "user_milestones",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_activities_user_id",
            table: "user_activities",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_wishlists_recipe_id",
            table: "recipe_wishlists",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_directions_recipe_id",
            table: "recipe_directions",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_created_by",
            table: "meal_plans",
            column: "created_by");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_recipes_recipe_id",
            table: "meal_plan_recipes",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_added_by",
            table: "food_items",
            column: "added_by");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_food_item_id",
            table: "food_item_reservations",
            column: "food_item_id");

        migrationBuilder.AddForeignKey(
            name: "fk_device_tokens_user_user_id",
            table: "device_tokens",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_logs_unit_unit_id",
            table: "food_item_logs",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_reservations_food_items_food_item_id",
            table: "food_item_reservations",
            column: "food_item_id",
            principalTable: "food_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_food_reference_food_reference_id",
            table: "food_items",
            column: "food_reference_id",
            principalTable: "food_references",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_unit_unit_id",
            table: "food_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_user_added_by",
            table: "food_items",
            column: "added_by",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_grocery_list_items_unit_unit_id",
            table: "grocery_list_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "fk_meal_plan_recipes_recipe_recipe_id",
            table: "meal_plan_recipes",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_meal_plans_user_created_by",
            table: "meal_plans",
            column: "created_by",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_notifications_user_user_id",
            table: "notifications",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_dietary_tags_recipes_recipe_id",
            table: "recipe_dietary_tags",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_directions_recipes_recipe_id",
            table: "recipe_directions",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_ingredients_unit_unit_id",
            table: "recipe_ingredients",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_wishlists_households_household_id",
            table: "recipe_wishlists",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_wishlists_recipes_recipe_id",
            table: "recipe_wishlists",
            column: "recipe_id",
            principalTable: "recipes",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_items_unit_unit_id",
            table: "trade_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_session_items_unit_unit_id",
            table: "trade_session_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_user_activities_user_user_id",
            table: "user_activities",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_milestones_users_user_id",
            table: "user_milestones",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_preferences_users_user_id",
            table: "user_preferences",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_device_tokens_user_user_id",
            table: "device_tokens");

        migrationBuilder.DropForeignKey(
            name: "fk_food_item_logs_unit_unit_id",
            table: "food_item_logs");

        migrationBuilder.DropForeignKey(
            name: "fk_food_item_reservations_food_items_food_item_id",
            table: "food_item_reservations");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_food_reference_food_reference_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_unit_unit_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_user_added_by",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_grocery_list_items_unit_unit_id",
            table: "grocery_list_items");

        migrationBuilder.DropForeignKey(
            name: "fk_meal_plan_recipes_recipe_recipe_id",
            table: "meal_plan_recipes");

        migrationBuilder.DropForeignKey(
            name: "fk_meal_plans_user_created_by",
            table: "meal_plans");

        migrationBuilder.DropForeignKey(
            name: "fk_notifications_user_user_id",
            table: "notifications");

        migrationBuilder.DropForeignKey(
            name: "fk_recipe_dietary_tags_recipes_recipe_id",
            table: "recipe_dietary_tags");

        migrationBuilder.DropForeignKey(
            name: "fk_recipe_directions_recipes_recipe_id",
            table: "recipe_directions");

        migrationBuilder.DropForeignKey(
            name: "fk_recipe_ingredients_unit_unit_id",
            table: "recipe_ingredients");

        migrationBuilder.DropForeignKey(
            name: "fk_recipe_wishlists_households_household_id",
            table: "recipe_wishlists");

        migrationBuilder.DropForeignKey(
            name: "fk_recipe_wishlists_recipes_recipe_id",
            table: "recipe_wishlists");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_items_unit_unit_id",
            table: "trade_items");

        migrationBuilder.DropForeignKey(
            name: "fk_trade_session_items_unit_unit_id",
            table: "trade_session_items");

        migrationBuilder.DropForeignKey(
            name: "fk_user_activities_user_user_id",
            table: "user_activities");

        migrationBuilder.DropForeignKey(
            name: "fk_user_milestones_users_user_id",
            table: "user_milestones");

        migrationBuilder.DropForeignKey(
            name: "fk_user_preferences_users_user_id",
            table: "user_preferences");

        migrationBuilder.DropIndex(
            name: "ix_user_milestones_user_id",
            table: "user_milestones");

        migrationBuilder.DropIndex(
            name: "ix_user_activities_user_id",
            table: "user_activities");

        migrationBuilder.DropIndex(
            name: "ix_recipe_wishlists_recipe_id",
            table: "recipe_wishlists");

        migrationBuilder.DropIndex(
            name: "ix_recipe_directions_recipe_id",
            table: "recipe_directions");

        migrationBuilder.DropIndex(
            name: "ix_meal_plans_created_by",
            table: "meal_plans");

        migrationBuilder.DropIndex(
            name: "ix_meal_plan_recipes_recipe_id",
            table: "meal_plan_recipes");

        migrationBuilder.DropIndex(
            name: "ix_food_items_added_by",
            table: "food_items");

        migrationBuilder.DropIndex(
            name: "ix_food_item_reservations_food_item_id",
            table: "food_item_reservations");

        migrationBuilder.CreateTable(
            name: "recipe_media",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                mime_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_media", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_media_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_recipe_media_recipe_id",
            table: "recipe_media",
            column: "recipe_id");

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_logs_units_unit_id",
            table: "food_item_logs",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_food_references_food_reference_id",
            table: "food_items",
            column: "food_reference_id",
            principalTable: "food_references",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_units_unit_id",
            table: "food_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_grocery_list_items_units_unit_id",
            table: "grocery_list_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_ingredients_units_unit_id",
            table: "recipe_ingredients",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_items_units_unit_id",
            table: "trade_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_trade_session_items_units_unit_id",
            table: "trade_session_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }
}
