using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveHasDataForUnit : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_recipe_ingredients_unit_unit_id",
            table: "recipe_ingredients");

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33d9-7b6d-8a94-4a8d5302e690"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-706e-a4ff-abd45a90c5b2"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7539-a4b0-eea241725e9f"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-759c-a006-a33e3c11e3b3"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-75ab-aa7a-a288318d69de"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-75e1-8ae3-b241bc0257a1"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7668-ad92-56f2885caf97"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7732-8cfd-270fdded32ef"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7978-9991-f4edad58f156"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7997-8d0a-fe037172427a"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-79b6-a5bd-78ccb65222ea"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7b3e-a34b-014e34049a1e"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7b54-8b57-5d2fd16eb89a"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7b5c-b9c7-fd3aaa72b0ea"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7c01-9566-36af00cd0d50"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7c52-8b79-146731e06c0b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7c92-ba4f-5481a772aa09"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7d82-88d8-4fbedd77c122"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7d93-afbc-3731c07b639c"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c9c-33da-7f79-8a49-0b8d3dfa82cd"));

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_ingredients_units_unit_id",
            table: "recipe_ingredients",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    private static readonly string[] columns = new[] { "id", "abbreviation", "is_archived", "is_deleted", "name", "to_base_factor", "type" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_recipe_ingredients_units_unit_id",
            table: "recipe_ingredients");

        migrationBuilder.InsertData(
            table: "units",
            columns: columns,
            values: new object[,]
            {
                { new Guid("019a4c9c-33d9-7b6d-8a94-4a8d5302e690"), "g", false, false, "Gram", 1m, "Weight" },
                { new Guid("019a4c9c-33da-706e-a4ff-abd45a90c5b2"), "doz", false, false, "Dozen", 12m, "Count" },
                { new Guid("019a4c9c-33da-7539-a4b0-eea241725e9f"), "lb", false, false, "Pound", 453.592m, "Weight" },
                { new Guid("019a4c9c-33da-759c-a006-a33e3c11e3b3"), "mL", false, false, "Millilitre", 1m, "Volume" },
                { new Guid("019a4c9c-33da-75ab-aa7a-a288318d69de"), "gal", false, false, "Gallon (US)", 3785.4m, "Volume" },
                { new Guid("019a4c9c-33da-75e1-8ae3-b241bc0257a1"), "qt", false, false, "Quart (US)", 946.35m, "Volume" },
                { new Guid("019a4c9c-33da-7668-ad92-56f2885caf97"), "kg", false, false, "Kilogram", 1000m, "Weight" },
                { new Guid("019a4c9c-33da-7732-8cfd-270fdded32ef"), "fl oz", false, false, "Fluid ounce (US)", 29.574m, "Volume" },
                { new Guid("019a4c9c-33da-7978-9991-f4edad58f156"), "pc", false, false, "Piece", 1m, "Count" },
                { new Guid("019a4c9c-33da-7997-8d0a-fe037172427a"), "oz", false, false, "Ounce", 28.3495m, "Weight" },
                { new Guid("019a4c9c-33da-79b6-a5bd-78ccb65222ea"), "mg", false, false, "Milligram", 0.001m, "Weight" },
                { new Guid("019a4c9c-33da-7b3e-a34b-014e34049a1e"), "serving", false, false, "Serving", 1m, "Count" },
                { new Guid("019a4c9c-33da-7b54-8b57-5d2fd16eb89a"), "tsp", false, false, "Teaspoon (US)", 5m, "Volume" },
                { new Guid("019a4c9c-33da-7b5c-b9c7-fd3aaa72b0ea"), "Tbsp", false, false, "Tablespoon (US)", 15m, "Volume" },
                { new Guid("019a4c9c-33da-7c01-9566-36af00cd0d50"), "gross", false, false, "Gross", 144m, "Count" },
                { new Guid("019a4c9c-33da-7c52-8b79-146731e06c0b"), "pair", false, false, "Pair", 2m, "Count" },
                { new Guid("019a4c9c-33da-7c92-ba4f-5481a772aa09"), "cup (US)", false, false, "Cup (US)", 240m, "Volume" },
                { new Guid("019a4c9c-33da-7d82-88d8-4fbedd77c122"), "pt", false, false, "Pint (US)", 473.2m, "Volume" },
                { new Guid("019a4c9c-33da-7d93-afbc-3731c07b639c"), "ea", false, false, "Each", 1m, "Count" },
                { new Guid("019a4c9c-33da-7f79-8a49-0b8d3dfa82cd"), "L", false, false, "Litre", 1000m, "Volume" }
            });

        migrationBuilder.AddForeignKey(
            name: "fk_recipe_ingredients_unit_unit_id",
            table: "recipe_ingredients",
            column: "unit_id",
            principalTable: "units",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
