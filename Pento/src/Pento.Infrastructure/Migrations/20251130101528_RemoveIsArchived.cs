using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveIsArchived : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "users");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "user_subscriptions");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "user_preferences");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "units");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "subscriptions");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "subscription_plans");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "subscription_features");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "storages");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipe_media");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipe_ingredients");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipe_directions");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipe_dietary_tags");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "payments");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "meal_plan_recipes");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "households");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "grocery_list_items");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "grocery_list_assignees");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "grocery_list");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "giveaway_posts");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "giveaway_claims");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "food_items");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "food_item_reservations");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "food_item_logs");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "food_dietary_tags");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "dietary_tags");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "compartments");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "comments");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "blog_posts");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "user_subscriptions",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "user_preferences",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "units",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "subscriptions",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "subscription_plans",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "subscription_features",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "storages",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "recipes",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "recipe_media",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "recipe_ingredients",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "recipe_directions",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "recipe_dietary_tags",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "payments",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "meal_plans",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "meal_plan_recipes",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "households",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "grocery_list_items",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "grocery_list_assignees",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "grocery_list",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "giveaway_posts",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "giveaway_claims",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "food_references",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "food_items",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "food_item_reservations",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "food_item_logs",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "food_dietary_tags",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "dietary_tags",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "compartments",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "comments",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "blog_posts",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
