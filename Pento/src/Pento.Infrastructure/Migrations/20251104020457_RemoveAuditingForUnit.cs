using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveAuditingForUnit : Migration
{
    private static readonly string[] columns = new[] { "id", "abbreviation", "is_archived", "is_deleted", "name", "to_base_factor", "type" };
    private static readonly string[] columnsArray = new[] { "id", "abbreviation", "created_at", "is_archived", "is_deleted", "name", "to_base_factor", "type", "updated_at" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
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

        migrationBuilder.DropColumn(
            name: "created_at",
            table: "units");

        migrationBuilder.DropColumn(
            name: "updated_at",
            table: "units");

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
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
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

        migrationBuilder.AddColumn<DateTime>(
            name: "created_at",
            table: "units",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_at",
            table: "units",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.InsertData(
            table: "units",
            columns: columnsArray,
            values: new object[,]
            {
                { new Guid("019a4c97-0a0f-7032-a168-a4f5bd1fef5d"), "ea", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9404), false, false, "Each", 1m, "Count", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9404) },
                { new Guid("019a4c97-0a0f-718c-bdcd-b5b91caf2812"), "pc", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9396), false, false, "Piece", 1m, "Count", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9396) },
                { new Guid("019a4c97-0a0f-72a2-8ebf-15c31bd70abc"), "pair", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9408), false, false, "Pair", 2m, "Count", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9408) },
                { new Guid("019a4c97-0a0f-7343-8482-42ec5b8ec960"), "oz", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9353), false, false, "Ounce", 28.3495m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9353) },
                { new Guid("019a4c97-0a0f-734a-bf66-12acce358d40"), "fl oz", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9370), false, false, "Fluid ounce (US)", 29.574m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9370) },
                { new Guid("019a4c97-0a0f-7378-a1db-67d98265640e"), "pt", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9377), false, false, "Pint (US)", 473.2m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9377) },
                { new Guid("019a4c97-0a0f-739d-a7b3-cda0ac97c962"), "gal", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9386), false, false, "Gallon (US)", 3785.4m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9386) },
                { new Guid("019a4c97-0a0f-740d-8b9b-2e9b01cda5dc"), "cup (US)", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9373), false, false, "Cup (US)", 240m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9373) },
                { new Guid("019a4c97-0a0f-744c-acb7-51f91fd984a3"), "qt", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9380), false, false, "Quart (US)", 946.35m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9380) },
                { new Guid("019a4c97-0a0f-7462-8dae-f0ad7cdc9d9b"), "kg", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9341), false, false, "Kilogram", 1000m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9341) },
                { new Guid("019a4c97-0a0f-751c-82a9-984618229a04"), "doz", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9411), false, false, "Dozen", 12m, "Count", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9411) },
                { new Guid("019a4c97-0a0f-76bd-941e-1867fa0374e5"), "mL", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9361), false, false, "Millilitre", 1m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9361) },
                { new Guid("019a4c97-0a0f-7838-9d4c-e051fa4faac9"), "lb", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9357), false, false, "Pound", 453.592m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9357) },
                { new Guid("019a4c97-0a0f-7846-a0cf-acc803e38176"), "mg", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9320), false, false, "Milligram", 0.001m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9320) },
                { new Guid("019a4c97-0a0f-7ac4-8f3e-5d916582bb97"), "Tbsp", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9392), false, false, "Tablespoon (US)", 15m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9392) },
                { new Guid("019a4c97-0a0f-7af8-85cf-f438bdeb8738"), "gross", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9416), false, false, "Gross", 144m, "Count", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9416) },
                { new Guid("019a4c97-0a0f-7b50-9306-ff348ec05a4f"), "g", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(3380), false, false, "Gram", 1m, "Weight", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(3380) },
                { new Guid("019a4c97-0a0f-7bea-8f13-189ad77eaaa5"), "serving", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9401), false, false, "Serving", 1m, "Count", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9401) },
                { new Guid("019a4c97-0a0f-7c52-a73c-eb7c3b707b75"), "tsp", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9389), false, false, "Teaspoon (US)", 5m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9389) },
                { new Guid("019a4c97-0a0f-7c99-b179-3bff77d18840"), "L", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9364), false, false, "Litre", 1000m, "Volume", new DateTime(2025, 11, 4, 1, 59, 18, 287, DateTimeKind.Utc).AddTicks(9364) }
            });
    }
}
