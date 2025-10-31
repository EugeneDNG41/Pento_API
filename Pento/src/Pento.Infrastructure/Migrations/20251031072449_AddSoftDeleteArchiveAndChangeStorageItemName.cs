using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddSoftDeleteArchiveAndChangeStorageItemName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_giveaway_posts_storage_item_storage_item_id",
            table: "giveaway_posts");

        migrationBuilder.DropTable(
            name: "storage_items");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Basic Member");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Grocery Runner");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Household Admin");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Storage Manager");

        migrationBuilder.RenameColumn(
            name: "storage_item_id",
            table: "giveaway_posts",
            newName: "food_item_id");

        migrationBuilder.RenameIndex(
            name: "ix_giveaway_posts_storage_item_id",
            table: "giveaway_posts",
            newName: "ix_giveaway_posts_food_item_id");

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "users",
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
            name: "is_deleted",
            table: "units",
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
            name: "is_deleted",
            table: "storages",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "type",
            table: "roles",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "recipes",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "recipes",
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
            name: "is_deleted",
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
            name: "is_deleted",
            table: "recipe_directions",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "possible_units",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "possible_units",
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
            name: "is_deleted",
            table: "meal_plans",
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
            name: "is_deleted",
            table: "households",
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
            name: "is_deleted",
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
            name: "is_deleted",
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
            name: "is_deleted",
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
            name: "is_deleted",
            table: "food_references",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<Guid>(
            name: "household_id",
            table: "compartments",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "compartments",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
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
            name: "is_deleted",
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

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "blog_posts",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "food_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                custom_name = table.Column<string>(type: "text", nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                expiration_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                source_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_items", x => x.id);
                table.ForeignKey(
                    name: "fk_food_items_compartments_compartment_id",
                    column: x => x.compartment_id,
                    principalTable: "compartments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_items_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_items_household_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.UpdateData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Administrator",
            column: "type",
            value: "System");

        migrationBuilder.UpdateData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Meal Planner",
            column: "type",
            value: "Household");

        migrationBuilder.UpdateData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Power Member",
            column: "type",
            value: "Household");

        migrationBuilder.InsertData(
            table: "roles",
            columns: ["name", "type"],
            values: new object[,]
            {
                { "Errand Runner", "Household" },
                { "Grocery Shopper", "Household" },
                { "Household Head", "Household" },
                { "Pantry Manager", "Household" }
            });

        migrationBuilder.CreateIndex(
            name: "ix_storages_household_id",
            table: "storages",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_compartments_household_id",
            table: "compartments",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_compartment_id",
            table: "food_items",
            column: "compartment_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_food_ref_id",
            table: "food_items",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_household_id",
            table: "food_items",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_unit_id",
            table: "food_items",
            column: "unit_id");

        migrationBuilder.AddForeignKey(
            name: "fk_compartments_household_household_id",
            table: "compartments",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_giveaway_posts_food_items_food_item_id",
            table: "giveaway_posts",
            column: "food_item_id",
            principalTable: "food_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_storages_households_household_id",
            table: "storages",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_compartments_household_household_id",
            table: "compartments");

        migrationBuilder.DropForeignKey(
            name: "fk_giveaway_posts_food_items_food_item_id",
            table: "giveaway_posts");

        migrationBuilder.DropForeignKey(
            name: "fk_storages_households_household_id",
            table: "storages");

        migrationBuilder.DropTable(
            name: "food_items");

        migrationBuilder.DropIndex(
            name: "ix_storages_household_id",
            table: "storages");

        migrationBuilder.DropIndex(
            name: "ix_compartments_household_id",
            table: "compartments");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Errand Runner");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Grocery Shopper");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Household Head");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Pantry Manager");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "users");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "users");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "units");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "units");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "storages");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "storages");

        migrationBuilder.DropColumn(
            name: "type",
            table: "roles");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipe_ingredients");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "recipe_ingredients");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "recipe_directions");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "recipe_directions");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "possible_units");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "possible_units");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "households");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "households");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "grocery_list");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "grocery_list");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "giveaway_posts");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "giveaway_posts");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "giveaway_claims");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "giveaway_claims");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "food_references");

        migrationBuilder.DropColumn(
            name: "household_id",
            table: "compartments");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "compartments");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "compartments");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "comments");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "comments");

        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "blog_posts");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "blog_posts");

        migrationBuilder.RenameColumn(
            name: "food_item_id",
            table: "giveaway_posts",
            newName: "storage_item_id");

        migrationBuilder.RenameIndex(
            name: "ix_giveaway_posts_food_item_id",
            table: "giveaway_posts",
            newName: "ix_giveaway_posts_storage_item_id");

        migrationBuilder.CreateTable(
            name: "storage_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                custom_name = table.Column<string>(type: "text", nullable: true),
                expiration_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                source_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_storage_items", x => x.id);
                table.ForeignKey(
                    name: "fk_storage_items_compartments_compartment_id",
                    column: x => x.compartment_id,
                    principalTable: "compartments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_storage_items_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_storage_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "roles",
            column: "name",
            values: new object[]
            {
                "Basic Member",
                "Grocery Runner",
                "Household Admin",
                "Storage Manager"
            });

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_compartment_id",
            table: "storage_items",
            column: "compartment_id");

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_food_ref_id",
            table: "storage_items",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_unit_id",
            table: "storage_items",
            column: "unit_id");

        migrationBuilder.AddForeignKey(
            name: "fk_giveaway_posts_storage_item_storage_item_id",
            table: "giveaway_posts",
            column: "storage_item_id",
            principalTable: "storage_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
