using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AlterActivity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "COMPARTMENT_QUANTITY");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "FOOD_ITEM_QUANTITY");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "STORAGE_QUANTITY");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "STORAGE_TYPE_FREEZER");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "STORAGE_TYPE_PANTRY");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "STORAGE_TYPE_REFRIGERATOR");

        migrationBuilder.DropColumn(
            name: "type",
            table: "activities");

        migrationBuilder.AddColumn<Guid>(
            name: "household_id",
            table: "user_activities",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_activities_household_id",
            table: "user_activities",
            column: "household_id");

        migrationBuilder.AddForeignKey(
            name: "fk_user_activities_households_household_id",
            table: "user_activities",
            column: "household_id",
            principalTable: "households",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    private static readonly string[] columns = new[] { "code", "description", "name", "type" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_user_activities_households_household_id",
            table: "user_activities");

        migrationBuilder.DropIndex(
            name: "ix_user_activities_household_id",
            table: "user_activities");

        migrationBuilder.DropColumn(
            name: "household_id",
            table: "user_activities");

        migrationBuilder.AddColumn<string>(
            name: "type",
            table: "activities",
            type: "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.UpdateData(
            table: "activities",
            keyColumn: "code",
            keyValue: "FOOD_ITEM_CONSUME",
            column: "type",
            value: "Action");

        migrationBuilder.UpdateData(
            table: "activities",
            keyColumn: "code",
            keyValue: "HOUSEHOLD_CREATE",
            column: "type",
            value: "Action");

        migrationBuilder.UpdateData(
            table: "activities",
            keyColumn: "code",
            keyValue: "STORAGE_CREATE",
            column: "type",
            value: "Action");

        migrationBuilder.InsertData(
            table: "activities",
            columns: columns,
            values: new object[,]
            {
                { "COMPARTMENT_QUANTITY", "Number of compartments.", "Compartment Quantity", "State" },
                { "FOOD_ITEM_QUANTITY", "Number of food items.", "Food Item Quantity", "State" },
                { "STORAGE_QUANTITY", "Number of storages.", "Storage Quantity", "State" },
                { "STORAGE_TYPE_FREEZER", "Number of freezer storages.", "Storage Type Freezer", "State" },
                { "STORAGE_TYPE_PANTRY", "Number of pantry storages.", "Storage Type Pantry", "State" },
                { "STORAGE_TYPE_REFRIGERATOR", "Number of refrigerator storages.", "Storage Type Refrigerator", "State" }
            });
    }
}
