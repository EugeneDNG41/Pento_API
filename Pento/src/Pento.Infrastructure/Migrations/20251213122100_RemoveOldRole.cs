using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveOldRole : Migration
{
    private static readonly string[] keyColumns = new[] { "permission_code", "role_name" };
    private static readonly string[] columns = new[] { "name", "type" };
    private static readonly string[] columnsArray = new[] { "permission_code", "role_name" };
    private static readonly string[] columnsArray0 = new[] { "name", "type" };
    private static readonly string[] columnsArray1 = new[] { "permission_code", "role_name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:delete", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:read", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:read", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:read", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:create", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:create", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:delete", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:read", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:read", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:read", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:update", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:delete", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:read", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:create", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:create", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:delete", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:read", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:read", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:read", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:update", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:update", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "household:read", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "household:read", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "household:read", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:create", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:delete", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:read", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:read", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:read", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:update", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "recipes:create", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "recipes:delete", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "recipes:read", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "recipes:update", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:delete", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:read", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:read", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:read", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "user:general", "User" });

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Grocery Shopper");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Meal Planner");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Pantry Manager");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "User");

        migrationBuilder.InsertData(
            table: "roles",
            columns: columnsArray0,
            values: new object[] { "Household Member", "Household" });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columnsArray1,
            values: new object[,]
            {
                { "compartments:create", "Household Member" },
                { "compartments:delete", "Household Member" },
                { "compartments:read", "Household Member" },
                { "compartments:update", "Household Member" },
                { "fooditems:create", "Household Member" },
                { "fooditems:delete", "Household Member" },
                { "fooditems:read", "Household Member" },
                { "fooditems:update", "Household Member" },
                { "giveaways:create", "Household Member" },
                { "giveaways:delete", "Household Member" },
                { "giveaways:update", "Household Member" },
                { "groceries:create", "Household Member" },
                { "groceries:delete", "Household Member" },
                { "groceries:read", "Household Member" },
                { "groceries:update", "Household Member" },
                { "household:read", "Household Member" },
                { "household:update", "Household Member" },
                { "mealplans:create", "Household Member" },
                { "mealplans:delete", "Household Member" },
                { "mealplans:read", "Household Member" },
                { "mealplans:update", "Household Member" },
                { "storages:create", "Household Member" },
                { "storages:delete", "Household Member" },
                { "storages:read", "Household Member" },
                { "storages:update", "Household Member" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:create", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:delete", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:read", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:create", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:delete", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:read", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "fooditems:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:create", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:delete", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "giveaways:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:create", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:delete", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:read", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "groceries:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "household:read", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "household:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:create", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:delete", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:read", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "mealplans:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:create", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:delete", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:read", "Household Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "storages:update", "Household Member" });

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Household Member");

        migrationBuilder.InsertData(
            table: "roles",
            columns: columns,
            values: new object[,]
            {
                { "Grocery Shopper", "Household" },
                { "Meal Planner", "Household" },
                { "Pantry Manager", "Household" },
                { "User", "General" }
            });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columnsArray,
            values: new object[,]
            {
                { "compartments:create", "Pantry Manager" },
                { "compartments:delete", "Pantry Manager" },
                { "compartments:read", "Grocery Shopper" },
                { "compartments:read", "Meal Planner" },
                { "compartments:read", "Pantry Manager" },
                { "compartments:update", "Pantry Manager" },
                { "fooditems:create", "Grocery Shopper" },
                { "fooditems:create", "Meal Planner" },
                { "fooditems:create", "Pantry Manager" },
                { "fooditems:delete", "Pantry Manager" },
                { "fooditems:read", "Grocery Shopper" },
                { "fooditems:read", "Meal Planner" },
                { "fooditems:read", "Pantry Manager" },
                { "fooditems:update", "Meal Planner" },
                { "fooditems:update", "Pantry Manager" },
                { "giveaways:create", "Pantry Manager" },
                { "giveaways:delete", "Pantry Manager" },
                { "giveaways:read", "User" },
                { "giveaways:update", "Pantry Manager" },
                { "groceries:create", "Grocery Shopper" },
                { "groceries:create", "Meal Planner" },
                { "groceries:create", "Pantry Manager" },
                { "groceries:delete", "Grocery Shopper" },
                { "groceries:read", "Grocery Shopper" },
                { "groceries:read", "Meal Planner" },
                { "groceries:read", "Pantry Manager" },
                { "groceries:update", "Grocery Shopper" },
                { "groceries:update", "Meal Planner" },
                { "groceries:update", "Pantry Manager" },
                { "household:read", "Grocery Shopper" },
                { "household:read", "Meal Planner" },
                { "household:read", "Pantry Manager" },
                { "mealplans:create", "Meal Planner" },
                { "mealplans:create", "Pantry Manager" },
                { "mealplans:delete", "Meal Planner" },
                { "mealplans:read", "Grocery Shopper" },
                { "mealplans:read", "Meal Planner" },
                { "mealplans:read", "Pantry Manager" },
                { "mealplans:update", "Meal Planner" },
                { "mealplans:update", "Pantry Manager" },
                { "recipes:create", "User" },
                { "recipes:delete", "User" },
                { "recipes:read", "User" },
                { "recipes:update", "User" },
                { "storages:create", "Pantry Manager" },
                { "storages:delete", "Pantry Manager" },
                { "storages:read", "Grocery Shopper" },
                { "storages:read", "Meal Planner" },
                { "storages:read", "Pantry Manager" },
                { "storages:update", "Pantry Manager" },
                { "user:general", "User" }
            });
    }
}
