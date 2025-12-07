using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UnitData : Migration
{
    private static readonly string[] columns = new[] { "id", "abbreviation", "created_at", "is_archived", "is_deleted", "name", "to_base_factor", "type", "updated_at" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_items_compartments_compartment_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_food_references_food_ref_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_household_household_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_food_items_unit_unit_id",
            table: "food_items");

        migrationBuilder.DropForeignKey(
            name: "fk_giveaway_posts_food_items_food_item_id",
            table: "giveaway_posts");

        migrationBuilder.DropTable(
            name: "grocery_list");

        migrationBuilder.DropPrimaryKey(
            name: "pk_food_items",
            table: "food_items");

        migrationBuilder.DropIndex(
            name: "ix_food_items_compartment_id",
            table: "food_items");

        migrationBuilder.DropIndex(
            name: "ix_food_items_household_id",
            table: "food_items");

        migrationBuilder.RenameTable(
            name: "food_items",
            newName: "food_item");

        migrationBuilder.RenameColumn(
            name: "custom_name",
            table: "food_item",
            newName: "name");

        migrationBuilder.RenameIndex(
            name: "ix_food_items_unit_id",
            table: "food_item",
            newName: "ix_food_item_unit_id");

        migrationBuilder.RenameIndex(
            name: "ix_food_items_food_ref_id",
            table: "food_item",
            newName: "ix_food_item_food_ref_id");

        migrationBuilder.AddColumn<string>(
            name: "type",
            table: "units",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<decimal>(
            name: "quantity",
            table: "food_item",
            type: "numeric",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(10,2)");

        migrationBuilder.AlterColumn<string>(
            name: "notes",
            table: "food_item",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            table: "food_item",
            type: "text",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "image_url",
            table: "food_item",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "version",
            table: "food_item",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddPrimaryKey(
            name: "pk_food_item",
            table: "food_item",
            column: "id");

        migrationBuilder.InsertData(
            table: "units",
            columns: columns,
            values: new object[,]
            {
                { new Guid("019a4c74-656d-7c4e-8a06-a047f8d99a19"), "g", new DateTime(2025, 11, 4, 1, 21, 27, 917, DateTimeKind.Utc).AddTicks(3920), false, false, "Gram", 1m, "Weight", new DateTime(2025, 11, 4, 1, 21, 27, 917, DateTimeKind.Utc).AddTicks(3920) },
                { new Guid("019a4c74-656e-72c5-8961-ad82b86d7c24"), "L", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(329), false, false, "Litre", 1000m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(329) },
                { new Guid("019a4c74-656e-72e8-88d7-a98aa830f534"), "cup (US)", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(336), false, false, "Cup (US)", 240m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(336) },
                { new Guid("019a4c74-656e-7464-a2ce-c3676d8e31a7"), "mg", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(288), false, false, "Milligram", 0.001m, "Weight", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(288) },
                { new Guid("019a4c74-656e-74f2-8cd8-fd1723eaf312"), "pt", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(341), false, false, "Pint (US)", 473.2m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(341) },
                { new Guid("019a4c74-656e-75ac-a853-4b323c7fcbe4"), "mL", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(325), false, false, "Millilitre", 1m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(325) },
                { new Guid("019a4c74-656e-75b1-9bc9-60ebce34b78a"), "fl oz", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(332), false, false, "Fluid ounce (US)", 29.574m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(332) },
                { new Guid("019a4c74-656e-7750-9ad7-ecf956c25e67"), "gal", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(348), false, false, "Gallon (US)", 3785.4m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(348) },
                { new Guid("019a4c74-656e-778e-b3be-1db0c0581e79"), "kg", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(309), false, false, "Kilogram", 1000m, "Weight", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(309) },
                { new Guid("019a4c74-656e-7968-b10e-18b14f7266e0"), "lb", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(318), false, false, "Pound", 453.592m, "Weight", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(318) },
                { new Guid("019a4c74-656e-79ae-b044-48e2ba8422d9"), "qt", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(345), false, false, "Quart (US)", 946.35m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(345) },
                { new Guid("019a4c74-656e-7aa0-9207-15d1374b9932"), "pc", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(360), false, false, "Piece", 1m, "Usage", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(360) },
                { new Guid("019a4c74-656e-7cc8-a4d2-a18e6eed6f74"), "pair", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(371), false, false, "Pair", 2m, "Usage", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(371) },
                { new Guid("019a4c74-656e-7cd7-8358-76d9cb6ccb75"), "doz", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(375), false, false, "Dozen", 12m, "Usage", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(375) },
                { new Guid("019a4c74-656e-7d2e-9941-d8985ff384c9"), "ea", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(366), false, false, "Each", 1m, "Usage", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(366) },
                { new Guid("019a4c74-656e-7e58-994a-697ebb361866"), "Tbsp", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(356), false, false, "Tablespoon (US)", 15m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(356) },
                { new Guid("019a4c74-656e-7ee0-b93a-10f275c3e86b"), "oz", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(314), false, false, "Ounce", 28.3495m, "Weight", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(314) },
                { new Guid("019a4c74-656e-7f7b-a511-9b9fbe29902b"), "gross", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(378), false, false, "Gross", 144m, "Usage", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(378) },
                { new Guid("019a4c74-656e-7f86-bfd5-3ddd12064a1b"), "tsp", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(351), false, false, "Teaspoon (US)", 5m, "Volume", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(351) },
                { new Guid("019a4c74-656e-7fea-ad82-1e352de24325"), "serving", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(363), false, false, "Serving", 1m, "Usage", new DateTime(2025, 11, 4, 1, 21, 27, 918, DateTimeKind.Utc).AddTicks(363) }
            });

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_food_references_food_ref_id",
            table: "food_item",
            column: "food_ref_id",
            principalTable: "food_references",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_food_item_units_unit_id",
            table: "food_item",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_giveaway_posts_food_item_food_item_id",
            table: "giveaway_posts",
            column: "food_item_id",
            principalTable: "food_item",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_item_food_references_food_ref_id",
            table: "food_item");

        migrationBuilder.DropForeignKey(
            name: "fk_food_item_units_unit_id",
            table: "food_item");

        migrationBuilder.DropForeignKey(
            name: "fk_giveaway_posts_food_item_food_item_id",
            table: "giveaway_posts");

        migrationBuilder.DropPrimaryKey(
            name: "pk_food_item",
            table: "food_item");

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656d-7c4e-8a06-a047f8d99a19"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-72c5-8961-ad82b86d7c24"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-72e8-88d7-a98aa830f534"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7464-a2ce-c3676d8e31a7"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-74f2-8cd8-fd1723eaf312"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-75ac-a853-4b323c7fcbe4"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-75b1-9bc9-60ebce34b78a"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7750-9ad7-ecf956c25e67"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-778e-b3be-1db0c0581e79"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7968-b10e-18b14f7266e0"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-79ae-b044-48e2ba8422d9"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7aa0-9207-15d1374b9932"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7cc8-a4d2-a18e6eed6f74"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7cd7-8358-76d9cb6ccb75"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7d2e-9941-d8985ff384c9"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7e58-994a-697ebb361866"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7ee0-b93a-10f275c3e86b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7f7b-a511-9b9fbe29902b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7f86-bfd5-3ddd12064a1b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c74-656e-7fea-ad82-1e352de24325"));

        migrationBuilder.DropColumn(
            name: "type",
            table: "units");

        migrationBuilder.DropColumn(
            name: "image_url",
            table: "food_item");

        migrationBuilder.DropColumn(
            name: "version",
            table: "food_item");

        migrationBuilder.RenameTable(
            name: "food_item",
            newName: "food_items");

        migrationBuilder.RenameColumn(
            name: "name",
            table: "food_items",
            newName: "custom_name");

        migrationBuilder.RenameIndex(
            name: "ix_food_item_unit_id",
            table: "food_items",
            newName: "ix_food_items_unit_id");

        migrationBuilder.RenameIndex(
            name: "ix_food_item_food_ref_id",
            table: "food_items",
            newName: "ix_food_items_food_ref_id");

        migrationBuilder.AlterColumn<decimal>(
            name: "quantity",
            table: "food_items",
            type: "numeric(10,2)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric");

        migrationBuilder.AlterColumn<string>(
            name: "notes",
            table: "food_items",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "custom_name",
            table: "food_items",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddPrimaryKey(
            name: "pk_food_items",
            table: "food_items",
            column: "id");

        migrationBuilder.CreateTable(
            name: "grocery_list",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_food_items_compartment_id",
            table: "food_items",
            column: "compartment_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_household_id",
            table: "food_items",
            column: "household_id");

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_compartments_compartment_id",
            table: "food_items",
            column: "compartment_id",
            principalTable: "compartments",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_food_references_food_ref_id",
            table: "food_items",
            column: "food_ref_id",
            principalTable: "food_references",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_household_household_id",
            table: "food_items",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_food_items_unit_unit_id",
            table: "food_items",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_giveaway_posts_food_items_food_item_id",
            table: "giveaway_posts",
            column: "food_item_id",
            principalTable: "food_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
