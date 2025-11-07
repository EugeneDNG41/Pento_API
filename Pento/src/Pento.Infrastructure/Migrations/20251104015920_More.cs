using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class More : Migration
{
    private static readonly string[] columns = new[] { "id", "abbreviation", "created_at", "is_archived", "is_deleted", "name", "to_base_factor", "type", "updated_at" };
    private static readonly string[] columnsArray = new[] { "id", "abbreviation", "created_at", "is_archived", "is_deleted", "name", "to_base_factor", "type", "updated_at" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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

        migrationBuilder.InsertData(
            table: "units",
            columns: columns,
            values: new object[,]
            {
                { new Guid("019a4c97-0a0f-7032-a168-a4f5bd1fef5d"), "ea", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9404), false, false, "Each", 1m, "Usage", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9404) },
                { new Guid("019a4c97-0a0f-718c-bdcd-b5b91caf2812"), "pc", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9396), false, false, "Piece", 1m, "Usage", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9396) },
                { new Guid("019a4c97-0a0f-72a2-8ebf-15c31bd70abc"), "pair", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9408), false, false, "Pair", 2m, "Usage", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9408) },
                { new Guid("019a4c97-0a0f-7343-8482-42ec5b8ec960"), "oz", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9353), false, false, "Ounce", 28.3495m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9353) },
                { new Guid("019a4c97-0a0f-734a-bf66-12acce358d40"), "fl oz", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9370), false, false, "Fluid ounce (US)", 29.574m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9370) },
                { new Guid("019a4c97-0a0f-7378-a1db-67d98265640e"), "pt", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9377), false, false, "Pint (US)", 473.2m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9377) },
                { new Guid("019a4c97-0a0f-739d-a7b3-cda0ac97c962"), "gal", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9386), false, false, "Gallon (US)", 3785.4m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9386) },
                { new Guid("019a4c97-0a0f-740d-8b9b-2e9b01cda5dc"), "cup (US)", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9373), false, false, "Cup (US)", 240m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9373) },
                { new Guid("019a4c97-0a0f-744c-acb7-51f91fd984a3"), "qt", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9380), false, false, "Quart (US)", 946.35m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9380) },
                { new Guid("019a4c97-0a0f-7462-8dae-f0ad7cdc9d9b"), "kg", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9341), false, false, "Kilogram", 1000m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9341) },
                { new Guid("019a4c97-0a0f-751c-82a9-984618229a04"), "doz", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9411), false, false, "Dozen", 12m, "Usage", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9411) },
                { new Guid("019a4c97-0a0f-76bd-941e-1867fa0374e5"), "mL", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9361), false, false, "Millilitre", 1m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9361) },
                { new Guid("019a4c97-0a0f-7838-9d4c-e051fa4faac9"), "lb", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9357), false, false, "Pound", 453.592m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9357) },
                { new Guid("019a4c97-0a0f-7846-a0cf-acc803e38176"), "mg", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9320), false, false, "Milligram", 0.001m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9320) },
                { new Guid("019a4c97-0a0f-7ac4-8f3e-5d916582bb97"), "Tbsp", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9392), false, false, "Tablespoon (US)", 15m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9392) },
                { new Guid("019a4c97-0a0f-7af8-85cf-f438bdeb8738"), "gross", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9416), false, false, "Gross", 144m, "Usage", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9416) },
                { new Guid("019a4c97-0a0f-7b50-9306-ff348ec05a4f"), "g", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(3380), false, false, "Gram", 1m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(3380) },
                { new Guid("019a4c97-0a0f-7bea-8f13-189ad77eaaa5"), "serving", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9401), false, false, "Serving", 1m, "Usage", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9401) },
                { new Guid("019a4c97-0a0f-7c52-a73c-eb7c3b707b75"), "tsp", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9389), false, false, "Teaspoon (US)", 5m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9389) },
                { new Guid("019a4c97-0a0f-7c99-b179-3bff77d18840"), "L", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9364), false, false, "Litre", 1000m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9364) }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7032-a168-a4f5bd1fef5d"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-718c-bdcd-b5b91caf2812"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-72a2-8ebf-15c31bd70abc"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7343-8482-42ec5b8ec960"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-734a-bf66-12acce358d40"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7378-a1db-67d98265640e"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-739d-a7b3-cda0ac97c962"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-740d-8b9b-2e9b01cda5dc"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-744c-acb7-51f91fd984a3"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7462-8dae-f0ad7cdc9d9b"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-751c-82a9-984618229a04"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-76bd-941e-1867fa0374e5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7838-9d4c-e051fa4faac9"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7846-a0cf-acc803e38176"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7ac4-8f3e-5d916582bb97"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7af8-85cf-f438bdeb8738"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7b50-9306-ff348ec05a4f"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7bea-8f13-189ad77eaaa5"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7c52-a73c-eb7c3b707b75"));

        migrationBuilder.DeleteData(
            table: "units",
            keyColumn: "id",
            keyValue: new Guid("019a4c97-0a0f-7c99-b179-3bff77d18840"));

        migrationBuilder.InsertData(
            table: "units",
            columns: columnsArray,
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
    }
}
