using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class New : Migration
{
    private static readonly string[] columns = new[] { "description", "name" };
    private static readonly string[] columnsArray = new[] { "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "compartments:update",
            column: "name",
            value: "Update Compartments");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "fooditems:update",
            column: "name",
            value: "Update Food Items");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "giveaways:update",
            column: "name",
            value: "Update Giveaways");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "groceries:update",
            column: "name",
            value: "Update Groceries");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "household:update",
            column: "description",
            value: "Update household name and invite code.");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "mealplans:update",
            column: "name",
            value: "Update Meal Plans");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "members:delete",
            columns: columns,
            values: new object[] { "Remove/kick members from the household and revoke their access.", "Remove Members" });

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "recipes:update",
            column: "name",
            value: "Update Recipes");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "storages:update",
            column: "name",
            value: "Update Storages");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "compartments:update",
            column: "name",
            value: "UpdateAsync Compartments");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "fooditems:update",
            column: "name",
            value: "UpdateAsync Food Items");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "giveaways:update",
            column: "name",
            value: "UpdateAsync Giveaways");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "groceries:update",
            column: "name",
            value: "UpdateAsync Groceries");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "household:update",
            column: "description",
            value: "UpdateAsync household name and invite code.");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "mealplans:update",
            column: "name",
            value: "UpdateAsync Meal Plans");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "members:delete",
            columns: columnsArray,
            values: new object[] { "RemoveAsync/kick members from the household and revoke their access.", "RemoveAsync Members" });

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "recipes:update",
            column: "name",
            value: "UpdateAsync Recipes");

        migrationBuilder.UpdateData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "storages:update",
            column: "name",
            value: "UpdateAsync Storages");
    }
}
