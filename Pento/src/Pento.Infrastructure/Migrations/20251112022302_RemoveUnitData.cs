using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveUnitData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7303-aa92-3f5139578124"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-73d9-af9e-7dbc62632fd5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-74f2-93af-1d2b4f0a5f2b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7514-84ac-2d56a46a6b68"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-753f-a9ac-909a53f81c32"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-75db-a692-24d77117f1e0"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-75e7-aa25-0092ac812d2b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7618-951f-63a9cacac0c5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7653-b76c-33eebae5380f"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7832-a3d3-695023e42c30"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-78d1-94b6-6fa719affd9f"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-792a-9555-96c0a0b06f3d"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-79ab-a57a-730252aa680f"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7af6-a939-707f31f48483"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7c4b-8ec0-2009de814e44"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7cca-8fe5-3f25071843e4"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7cff-aacb-5bb1bf0dcc27"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7d00-b5fd-f84224134fa5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7dca-867c-cd66e474cae0"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a75d8-2ed9-7dec-8cb4-db4697fbd363"));
    }

    private static readonly string[] columns = new[] { "id", "abbreviation", "is_archived", "is_deleted", "name", "to_base_factor", "type" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "units",
            columns: columns,
            values: new object[,]
            {
                { new Guid("019a75d8-2ed9-7303-aa92-3f5139578124"), "serving", false, false, "Serving", 1m, "Count" },
                { new Guid("019a75d8-2ed9-73d9-af9e-7dbc62632fd5"), "pair", false, false, "Pair", 2m, "Count" },
                { new Guid("019a75d8-2ed9-74f2-93af-1d2b4f0a5f2b"), "pt", false, false, "Pint (US)", 473.2m, "Volume" },
                { new Guid("019a75d8-2ed9-7514-84ac-2d56a46a6b68"), "mL", false, false, "Millilitre", 1m, "Volume" },
                { new Guid("019a75d8-2ed9-753f-a9ac-909a53f81c32"), "oz", false, false, "Ounce", 28.3495m, "Weight" },
                { new Guid("019a75d8-2ed9-75db-a692-24d77117f1e0"), "fl oz", false, false, "Fluid ounce (US)", 29.574m, "Volume" },
                { new Guid("019a75d8-2ed9-75e7-aa25-0092ac812d2b"), "Tbsp", false, false, "Tablespoon (US)", 15m, "Volume" },
                { new Guid("019a75d8-2ed9-7618-951f-63a9cacac0c5"), "gross", false, false, "Gross", 144m, "Count" },
                { new Guid("019a75d8-2ed9-7653-b76c-33eebae5380f"), "cup (US)", false, false, "Cup (US)", 240m, "Volume" },
                { new Guid("019a75d8-2ed9-7832-a3d3-695023e42c30"), "ea", false, false, "Each", 1m, "Count" },
                { new Guid("019a75d8-2ed9-78d1-94b6-6fa719affd9f"), "kg", false, false, "Kilogram", 1000m, "Weight" },
                { new Guid("019a75d8-2ed9-792a-9555-96c0a0b06f3d"), "mg", false, false, "Milligram", 0.001m, "Weight" },
                { new Guid("019a75d8-2ed9-79ab-a57a-730252aa680f"), "qt", false, false, "Quart (US)", 946.35m, "Volume" },
                { new Guid("019a75d8-2ed9-7af6-a939-707f31f48483"), "gal", false, false, "Gallon (US)", 3785.4m, "Volume" },
                { new Guid("019a75d8-2ed9-7c4b-8ec0-2009de814e44"), "lb", false, false, "Pound", 453.592m, "Weight" },
                { new Guid("019a75d8-2ed9-7cca-8fe5-3f25071843e4"), "g", false, false, "Gram", 1m, "Weight" },
                { new Guid("019a75d8-2ed9-7cff-aacb-5bb1bf0dcc27"), "pc", false, false, "Piece", 1m, "Count" },
                { new Guid("019a75d8-2ed9-7d00-b5fd-f84224134fa5"), "doz", false, false, "Dozen", 12m, "Count" },
                { new Guid("019a75d8-2ed9-7dca-867c-cd66e474cae0"), "tsp", false, false, "Teaspoon (US)", 5m, "Volume" },
                { new Guid("019a75d8-2ed9-7dec-8cb4-db4697fbd363"), "L", false, false, "Litre", 1000m, "Volume" }
            });
    }
}
