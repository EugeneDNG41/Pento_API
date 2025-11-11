using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddUnitData : Migration
{
    private static readonly string[] columns = new[] { "id", "abbreviation", "is_archived", "is_deleted", "name", "to_base_factor", "type" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "units",
            columns: columns,
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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
