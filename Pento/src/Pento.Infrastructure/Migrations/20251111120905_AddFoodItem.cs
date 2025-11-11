using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFoodItem : Migration
{
    private static readonly string[] columns = new[] { "id", "abbreviation", "is_archived", "is_deleted", "name", "to_base_factor", "type" };
    private static readonly string[] columnsArray = new[] { "id", "abbreviation", "is_archived", "is_deleted", "name", "to_base_factor", "type" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-70a4-97bd-49ffc419b6c9"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-72ff-9fce-10da157e462a"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7400-be11-3981abc39f09"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-74b1-bc45-3fa37d35dd9c"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-74ca-b47f-bfc3247bb7ef"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-74cb-bab3-22826d12bd8b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7522-8216-ef1ad1dc813a"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7719-b0da-8aac472cf0f7"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-785d-a459-d738757017ac"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-78df-b39c-ccf24b46ff5c"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-79b1-9035-3a08c9f630c0"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7a8e-96e7-bbe01ecc0515"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7ad6-8425-f0a0eb6b5ea5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7b42-8006-61dbed3441f3"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7b42-a8a2-a60655f347d5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7bfb-b2d4-f027df22cd8b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7d08-9613-59b8cdec9d8c"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7e9e-93fc-d560e253f89c"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7ef1-9d84-83ae2c3ca50d"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a6d86-5225-7fa7-bb57-a778307838a3"));

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "storages",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "meal_plans",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "compartments",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.CreateTable(
            name: "food_item_reservations",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                reservation_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                reservation_for = table.Column<int>(type: "integer", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                giveaway_post_id = table.Column<Guid>(type: "uuid", nullable: true),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_item_reservations", x => x.id);
                table.ForeignKey(
                    name: "fk_food_item_reservations_giveaway_post_giveaway_post_id",
                    column: x => x.giveaway_post_id,
                    principalTable: "giveaway_posts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_reservations_meal_plan_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_reservations_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "food_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                expiration_date = table.Column<DateOnly>(type: "date", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                added_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
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
                    name: "fk_food_items_food_references_food_reference_id",
                    column: x => x.food_reference_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_food_items_household_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_items_units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "food_item_logs",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                action = table.Column<int>(type: "integer", nullable: false),
                base_quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                base_unit_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_item_logs", x => x.id);
                table.ForeignKey(
                    name: "fk_food_item_logs_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_logs_household_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_logs_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "units",
            columns: columns,
            values: new object[,]
            {
                { new Guid("019a72d1-cf22-7400-8f1c-85aac238e180"), "g", false, false, "Gram", 1m, "Weight" },
                { new Guid("019a72d1-cf23-72a2-a4b4-77bd62fd8de9"), "pair", false, false, "Pair", 2m, "Count" },
                { new Guid("019a72d1-cf23-73bf-a9e4-2064d9c56c60"), "Tbsp", false, false, "Tablespoon (US)", 15m, "Volume" },
                { new Guid("019a72d1-cf23-7450-86e7-1338d5aca9db"), "oz", false, false, "Ounce", 28.3495m, "Weight" },
                { new Guid("019a72d1-cf23-7521-8117-cc70fe844afb"), "qt", false, false, "Quart (US)", 946.35m, "Volume" },
                { new Guid("019a72d1-cf23-758e-98f3-13ca7e9c3c68"), "pc", false, false, "Piece", 1m, "Count" },
                { new Guid("019a72d1-cf23-75e5-825a-ec24fdffcc17"), "lb", false, false, "Pound", 453.592m, "Weight" },
                { new Guid("019a72d1-cf23-774f-bef2-1318caf354d2"), "ea", false, false, "Each", 1m, "Count" },
                { new Guid("019a72d1-cf23-777b-9fe9-ef5e685a69b3"), "serving", false, false, "Serving", 1m, "Count" },
                { new Guid("019a72d1-cf23-7814-9a0b-63265469720b"), "gal", false, false, "Gallon (US)", 3785.4m, "Volume" },
                { new Guid("019a72d1-cf23-7816-8162-b7856f234a16"), "mL", false, false, "Millilitre", 1m, "Volume" },
                { new Guid("019a72d1-cf23-7a24-857f-005b1837cdaf"), "kg", false, false, "Kilogram", 1000m, "Weight" },
                { new Guid("019a72d1-cf23-7a55-bb6f-8baae55ff930"), "tsp", false, false, "Teaspoon (US)", 5m, "Volume" },
                { new Guid("019a72d1-cf23-7cc2-b4ab-b52a6476df81"), "mg", false, false, "Milligram", 0.001m, "Weight" },
                { new Guid("019a72d1-cf23-7d3a-a383-a15a0d311bd9"), "gross", false, false, "Gross", 144m, "Count" },
                { new Guid("019a72d1-cf23-7d56-b203-e9211a170627"), "pt", false, false, "Pint (US)", 473.2m, "Volume" },
                { new Guid("019a72d1-cf23-7d86-9256-78cd8c05bb21"), "cup (US)", false, false, "Cup (US)", 240m, "Volume" },
                { new Guid("019a72d1-cf23-7e16-9ba5-fcab61f7bafa"), "doz", false, false, "Dozen", 12m, "Count" },
                { new Guid("019a72d1-cf23-7ef3-a336-7fcaffeee62d"), "fl oz", false, false, "Fluid ounce (US)", 29.574m, "Volume" },
                { new Guid("019a72d1-cf23-7f4f-a749-011d205c891c"), "L", false, false, "Litre", 1000m, "Volume" }
            });

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_food_item_id",
            table: "food_item_logs",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_household_id",
            table: "food_item_logs",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_user_id",
            table: "food_item_logs",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_giveaway_post_id",
            table: "food_item_reservations",
            column: "giveaway_post_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_meal_plan_id",
            table: "food_item_reservations",
            column: "meal_plan_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_recipe_id",
            table: "food_item_reservations",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_compartment_id",
            table: "food_items",
            column: "compartment_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_food_reference_id",
            table: "food_items",
            column: "food_reference_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_household_id",
            table: "food_items",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_unit_id",
            table: "food_items",
            column: "unit_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "food_item_logs");

        migrationBuilder.DropTable(
            name: "food_item_reservations");

        migrationBuilder.DropTable(
            name: "food_items");

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf22-7400-8f1c-85aac238e180"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-72a2-a4b4-77bd62fd8de9"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-73bf-a9e4-2064d9c56c60"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7450-86e7-1338d5aca9db"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7521-8117-cc70fe844afb"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-758e-98f3-13ca7e9c3c68"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-75e5-825a-ec24fdffcc17"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-774f-bef2-1318caf354d2"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-777b-9fe9-ef5e685a69b3"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7814-9a0b-63265469720b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7816-8162-b7856f234a16"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7a24-857f-005b1837cdaf"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7a55-bb6f-8baae55ff930"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7cc2-b4ab-b52a6476df81"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7d3a-a383-a15a0d311bd9"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7d56-b203-e9211a170627"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7d86-9256-78cd8c05bb21"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7e16-9ba5-fcab61f7bafa"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7ef3-a336-7fcaffeee62d"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a72d1-cf23-7f4f-a749-011d205c891c"));

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "storages");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "meal_plans");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "compartments");

        migrationBuilder.InsertData(
            table: "units",
            columns: columnsArray,
            values: new object[,]
            {
                { new Guid("019a6d86-5225-70a4-97bd-49ffc419b6c9"), "pt", false, false, "Pint (US)", 473.2m, "Volume" },
                { new Guid("019a6d86-5225-72ff-9fce-10da157e462a"), "lb", false, false, "Pound", 453.592m, "Weight" },
                { new Guid("019a6d86-5225-7400-be11-3981abc39f09"), "gross", false, false, "Gross", 144m, "Count" },
                { new Guid("019a6d86-5225-74b1-bc45-3fa37d35dd9c"), "pair", false, false, "Pair", 2m, "Count" },
                { new Guid("019a6d86-5225-74ca-b47f-bfc3247bb7ef"), "ea", false, false, "Each", 1m, "Count" },
                { new Guid("019a6d86-5225-74cb-bab3-22826d12bd8b"), "doz", false, false, "Dozen", 12m, "Count" },
                { new Guid("019a6d86-5225-7522-8216-ef1ad1dc813a"), "cup (US)", false, false, "Cup (US)", 240m, "Volume" },
                { new Guid("019a6d86-5225-7719-b0da-8aac472cf0f7"), "kg", false, false, "Kilogram", 1000m, "Weight" },
                { new Guid("019a6d86-5225-785d-a459-d738757017ac"), "serving", false, false, "Serving", 1m, "Count" },
                { new Guid("019a6d86-5225-78df-b39c-ccf24b46ff5c"), "mg", false, false, "Milligram", 0.001m, "Weight" },
                { new Guid("019a6d86-5225-79b1-9035-3a08c9f630c0"), "pc", false, false, "Piece", 1m, "Count" },
                { new Guid("019a6d86-5225-7a8e-96e7-bbe01ecc0515"), "oz", false, false, "Ounce", 28.3495m, "Weight" },
                { new Guid("019a6d86-5225-7ad6-8425-f0a0eb6b5ea5"), "qt", false, false, "Quart (US)", 946.35m, "Volume" },
                { new Guid("019a6d86-5225-7b42-8006-61dbed3441f3"), "fl oz", false, false, "Fluid ounce (US)", 29.574m, "Volume" },
                { new Guid("019a6d86-5225-7b42-a8a2-a60655f347d5"), "gal", false, false, "Gallon (US)", 3785.4m, "Volume" },
                { new Guid("019a6d86-5225-7bfb-b2d4-f027df22cd8b"), "tsp", false, false, "Teaspoon (US)", 5m, "Volume" },
                { new Guid("019a6d86-5225-7d08-9613-59b8cdec9d8c"), "L", false, false, "Litre", 1000m, "Volume" },
                { new Guid("019a6d86-5225-7e9e-93fc-d560e253f89c"), "Tbsp", false, false, "Tablespoon (US)", 15m, "Volume" },
                { new Guid("019a6d86-5225-7ef1-9d84-83ae2c3ca50d"), "g", false, false, "Gram", 1m, "Weight" },
                { new Guid("019a6d86-5225-7fa7-bb57-a778307838a3"), "mL", false, false, "Millilitre", 1m, "Volume" }
            });
    }
}
