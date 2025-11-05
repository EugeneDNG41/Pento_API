using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddUserRole : Migration
{
    private static readonly string[] keyColumns = new[] { "permission_code", "role_name" };
    private static readonly string[] keyColumnsArray = new[] { "permission_code", "role_name" };
    private static readonly string[] columns = new[] { "name", "type" };
    private static readonly string[] columnsArray = new[] { "code", "description", "name" };
    private static readonly string[] columnsArray0 = new[] { "name", "type" };


    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumns,
            keyValues: new object[] { "compartments:create", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:create", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:create", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:create", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:delete", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:read", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:read", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "compartments:update", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:create", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:create", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:delete", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:read", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:read", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:update", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:create", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:create", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:delete", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:read", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:read", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:update", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "household:read", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "household:read", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "household:update", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:create", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:create", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:create", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:delete", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:read", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:read", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:update", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:create", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:create", "Grocery Shopper" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:create", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:create", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:delete", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:read", "Errand Runner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:read", "Power Member" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "storages:update", "Power Member" });

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Errand Runner");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Power Member");

        migrationBuilder.InsertData(
            table: "permissions",
            columns: columnsArray,
            values: new object[,]
            {
                { "giveaways:create", "Create giveaway posts for surplus food items.", "Create Giveaways" },
                { "giveaways:delete", "Delete giveaway posts you created.", "Delete Giveaways" },
                { "giveaways:read", "View giveaway posts and details. Read-only.", "View Giveaways" },
                { "giveaways:update", "Edit giveaway posts you created.", "Update Giveaways" },
                { "recipes:create", "Create new recipes.", "Add Recipes" },
                { "recipes:delete", "Delete your own recipes.", "Delete Recipes" },
                { "recipes:read", "View recipes and their details. Read-only.", "View Recipes" },
                { "recipes:update", "edit your own recipes.", "Update Recipes" },
                { "user:general", "Basic user access and functionality.", "User General" }
            });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: keyColumns,
            values: new object[,]
            {
                { "fooditems:update", "Meal Planner" },
                { "groceries:update", "Meal Planner" },
                { "groceries:update", "Pantry Manager" },
                { "mealplans:update", "Pantry Manager" }
            });

        migrationBuilder.UpdateData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Administrator",
            column: "type",
            value: "Administrative");

        migrationBuilder.InsertData(
            table: "roles",
            columns: columns,
            values: new object[] { "User", "General" });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: keyColumns,
            values: new object[,]
            {
                { "giveaways:create", "Household Head" },
                { "giveaways:create", "Pantry Manager" },
                { "giveaways:delete", "Household Head" },
                { "giveaways:delete", "Pantry Manager" },
                { "giveaways:read", "User" },
                { "giveaways:update", "Household Head" },
                { "giveaways:update", "Pantry Manager" },
                { "recipes:create", "User" },
                { "recipes:delete", "User" },
                { "recipes:read", "User" },
                { "recipes:update", "User" },
                { "user:general", "User" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "fooditems:update", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:create", "Household Head" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:create", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:delete", "Household Head" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:delete", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:read", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:update", "Household Head" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "giveaways:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:update", "Meal Planner" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "groceries:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "mealplans:update", "Pantry Manager" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "recipes:create", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "recipes:delete", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "recipes:read", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "recipes:update", "User" });

        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: keyColumnsArray,
            keyValues: new object[] { "user:general", "User" });

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "giveaways:create");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "giveaways:delete");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "giveaways:read");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "giveaways:update");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "recipes:create");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "recipes:delete");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "recipes:read");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "recipes:update");

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "user:general");

        migrationBuilder.DeleteData(
            table: "roles",
            keyColumn: "name",
            keyValue: "User");

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: keyColumns,
            values: new object[,]
            {
                { "compartments:create", "Grocery Shopper" },
                { "compartments:create", "Meal Planner" },
                { "mealplans:create", "Grocery Shopper" },
                { "storages:create", "Grocery Shopper" },
                { "storages:create", "Meal Planner" }
            });

        migrationBuilder.UpdateData(
            table: "roles",
            keyColumn: "name",
            keyValue: "Administrator",
            column: "type",
            value: "System");

        migrationBuilder.InsertData(
            table: "roles",
            columns: columnsArray0,
            values: new object[,]
            {
                { "Errand Runner", "Household" },
                { "Power Member", "Household" }
            });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: keyColumns,
            values: new object[,]
            {
                { "compartments:create", "Errand Runner" },
                { "compartments:create", "Power Member" },
                { "compartments:delete", "Power Member" },
                { "compartments:read", "Errand Runner" },
                { "compartments:read", "Power Member" },
                { "compartments:update", "Power Member" },
                { "fooditems:create", "Errand Runner" },
                { "fooditems:create", "Power Member" },
                { "fooditems:delete", "Power Member" },
                { "fooditems:read", "Errand Runner" },
                { "fooditems:read", "Power Member" },
                { "fooditems:update", "Power Member" },
                { "groceries:create", "Errand Runner" },
                { "groceries:create", "Power Member" },
                { "groceries:delete", "Power Member" },
                { "groceries:read", "Errand Runner" },
                { "groceries:read", "Power Member" },
                { "groceries:update", "Power Member" },
                { "household:read", "Errand Runner" },
                { "household:read", "Power Member" },
                { "household:update", "Power Member" },
                { "mealplans:create", "Errand Runner" },
                { "mealplans:create", "Power Member" },
                { "mealplans:delete", "Power Member" },
                { "mealplans:read", "Errand Runner" },
                { "mealplans:read", "Power Member" },
                { "mealplans:update", "Power Member" },
                { "storages:create", "Errand Runner" },
                { "storages:create", "Power Member" },
                { "storages:delete", "Power Member" },
                { "storages:read", "Errand Runner" },
                { "storages:read", "Power Member" },
                { "storages:update", "Power Member" }
            });
    }
}
