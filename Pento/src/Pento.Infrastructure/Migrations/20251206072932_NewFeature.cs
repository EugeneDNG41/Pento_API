using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class NewFeature : Migration
{
    private static readonly string[] columns = new[] { "code", "default_quota", "default_reset_period", "description", "name" };
    private static readonly string[] columnsArray = new[] { "code", "default_quota", "default_reset_period", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyValue: "MEAL_PLAN_SLOT");

        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyValue: "STORAGE_SLOT");

        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            table: "households",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AlterColumn<string>(
            name: "platform",
            table: "device_tokens",
            type: "text",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.InsertData(
            table: "features",
            columns: columns,
            values: new object[] { "GROCERY_MAP", null, null, "Show grocery options nearby on google map.", "Grocery Map" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "features",
            keyColumn: "code",
            keyValue: "GROCERY_MAP");

        migrationBuilder.DropColumn(
            name: "created_on",
            table: "households");

        migrationBuilder.AlterColumn<int>(
            name: "platform",
            table: "device_tokens",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.InsertData(
            table: "features",
            columns: columnsArray,
            values: new object[,]
            {
                { "MEAL_PLAN_SLOT", 5, null, "Total meal plan slot for scheduling and tracking meals.", "Meal Plan Slot" },
                { "STORAGE_SLOT", 5, null, "Total storage slots for pantry management.", "Storage Slot" }
            });
    }
}
