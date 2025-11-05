using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Permission : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };
    private static readonly string[] columnsArray = new[] { "permission_code", "role_name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "permissions",
            columns: table => new
            {
                code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_permissions", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "role_permissions",
            columns: table => new
            {
                permission_code = table.Column<string>(type: "character varying(100)", nullable: false),
                role_name = table.Column<string>(type: "character varying(50)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_permissions", x => new { x.permission_code, x.role_name });
                table.ForeignKey(
                    name: "fk_role_permissions_permissions_permission_code",
                    column: x => x.permission_code,
                    principalTable: "permissions",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_role_permissions_roles_role_name",
                    column: x => x.role_name,
                    principalTable: "roles",
                    principalColumn: "name",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "permissions",
            columns: columns,
            values: new object[,]
            {
                { "compartments:create", "Create compartments within a storage (shelves, bins, drawers).", "Add Compartments" },
                { "compartments:delete", "Delete compartments. Usually blocked if they still contain items.", "Delete Compartments" },
                { "compartments:read", "View compartments/shelves within a storage. Read-only.", "View Compartments" },
                { "compartments:update", "Rename/reorder compartments and edit their attributes.", "Update Compartments" },
                { "fooditems:create", "Add new items to inventory.", "Add Food Items" },
                { "fooditems:delete", "Delete/remove items.", "Delete Food Items" },
                { "fooditems:read", "View inventory items, quantities, and expirations. Read-only.", "View Food Items" },
                { "fooditems:update", "Edit item details and adjust quantities (consume/waste/donate).", "Update Food Items" },
                { "foodreferences:manage", "Manage the authoritative food reference catalog.", "Manage Food References" },
                { "giveaways:manage", "Create/update/approve/close giveaway posts and moderate entries.", "Manage Giveaways" },
                { "groceries:create", "Create grocery lists and add items to lists.", "Add Groceries" },
                { "groceries:delete", "Delete grocery lists and/or list items.", "Delete Groceries" },
                { "groceries:read", "View grocery lists and list items. Read-only.", "View Groceries" },
                { "groceries:update", "Edit grocery lists and list items.", "Update Groceries" },
                { "household:read", "View the current household’s profile, settings, and membership. Read-only.", "View Household" },
                { "household:update", "Update household name and invite code.", "Manage Household" },
                { "households:manage", "Create/update/merge/archive households at the system level.", "Manage Households" },
                { "households:read", "View all households across the system. Read-only.", "View Households" },
                { "mealplans:create", "Create meal plans and add meals/recipes to the schedule.", "Add Meal Plans" },
                { "mealplans:delete", "Delete meal plans.", "Delete Meal Plans" },
                { "mealplans:read", "View meal plans and scheduled recipes. Read-only.", "View Meal Plans" },
                { "mealplans:update", "Modify meal plans.", "Update Meal Plans" },
                { "members:delete", "Remove/kick members from the household and revoke their access.", "Remove Members" },
                { "members:update", "Change member roles within the household.", "Manage Members" },
                { "permissions:read", "View the catalog of permissions and their descriptions. Read-only.", "View Permissions" },
                { "recipes:manage", "Create/update/delete recipes and moderate community submissions.", "Manage Recipes" },
                { "roles:manage", "Create/update/delete roles and assign/unassign permissions to roles.", "Manage Roles" },
                { "roles:read", "View roles and their permission mappings. Read-only.", "View Roles" },
                { "storages:create", "Create new storage locations under the household.", "Add Storages" },
                { "storages:delete", "Delete a storage. Typically requires it to be empty; irreversible.", "Delete Storage" },
                { "storages:read", "List and view all storages (pantry, fridge, etc.) and their attributes. Read-only.", "View Storages" },
                { "storages:update", "Rename storages and modify their attributes.", "Update Storages" },
                { "users:delete", "Delete/deactivate user accounts according to policy.", "Delete Users" },
                { "users:manage", "Create/update users or change status (e.g., lock/enable). Excludes hard delete.", "Manage Users" },
                { "users:read", "View user accounts and basic profile/usage data. Read-only.", "View Users" }
            });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columnsArray,
            values: new object[,]
            {
                { "compartments:create", "Errand Runner" },
                { "compartments:create", "Grocery Shopper" },
                { "compartments:create", "Household Head" },
                { "compartments:create", "Meal Planner" },
                { "compartments:create", "Pantry Manager" },
                { "compartments:create", "Power Member" },
                { "compartments:delete", "Household Head" },
                { "compartments:delete", "Pantry Manager" },
                { "compartments:delete", "Power Member" },
                { "compartments:read", "Errand Runner" },
                { "compartments:read", "Grocery Shopper" },
                { "compartments:read", "Household Head" },
                { "compartments:read", "Meal Planner" },
                { "compartments:read", "Pantry Manager" },
                { "compartments:read", "Power Member" },
                { "compartments:update", "Household Head" },
                { "compartments:update", "Pantry Manager" },
                { "compartments:update", "Power Member" },
                { "fooditems:create", "Errand Runner" },
                { "fooditems:create", "Grocery Shopper" },
                { "fooditems:create", "Household Head" },
                { "fooditems:create", "Meal Planner" },
                { "fooditems:create", "Pantry Manager" },
                { "fooditems:create", "Power Member" },
                { "fooditems:delete", "Household Head" },
                { "fooditems:delete", "Pantry Manager" },
                { "fooditems:delete", "Power Member" },
                { "fooditems:read", "Errand Runner" },
                { "fooditems:read", "Grocery Shopper" },
                { "fooditems:read", "Household Head" },
                { "fooditems:read", "Meal Planner" },
                { "fooditems:read", "Pantry Manager" },
                { "fooditems:read", "Power Member" },
                { "fooditems:update", "Household Head" },
                { "fooditems:update", "Pantry Manager" },
                { "fooditems:update", "Power Member" },
                { "foodreferences:manage", "Administrator" },
                { "giveaways:manage", "Administrator" },
                { "groceries:create", "Errand Runner" },
                { "groceries:create", "Grocery Shopper" },
                { "groceries:create", "Household Head" },
                { "groceries:create", "Meal Planner" },
                { "groceries:create", "Pantry Manager" },
                { "groceries:create", "Power Member" },
                { "groceries:delete", "Grocery Shopper" },
                { "groceries:delete", "Household Head" },
                { "groceries:delete", "Power Member" },
                { "groceries:read", "Errand Runner" },
                { "groceries:read", "Grocery Shopper" },
                { "groceries:read", "Household Head" },
                { "groceries:read", "Meal Planner" },
                { "groceries:read", "Pantry Manager" },
                { "groceries:read", "Power Member" },
                { "groceries:update", "Grocery Shopper" },
                { "groceries:update", "Household Head" },
                { "groceries:update", "Power Member" },
                { "household:read", "Errand Runner" },
                { "household:read", "Grocery Shopper" },
                { "household:read", "Household Head" },
                { "household:read", "Meal Planner" },
                { "household:read", "Pantry Manager" },
                { "household:read", "Power Member" },
                { "household:update", "Household Head" },
                { "household:update", "Power Member" },
                { "households:manage", "Administrator" },
                { "households:read", "Administrator" },
                { "mealplans:create", "Errand Runner" },
                { "mealplans:create", "Grocery Shopper" },
                { "mealplans:create", "Household Head" },
                { "mealplans:create", "Meal Planner" },
                { "mealplans:create", "Pantry Manager" },
                { "mealplans:create", "Power Member" },
                { "mealplans:delete", "Household Head" },
                { "mealplans:delete", "Meal Planner" },
                { "mealplans:delete", "Power Member" },
                { "mealplans:read", "Errand Runner" },
                { "mealplans:read", "Grocery Shopper" },
                { "mealplans:read", "Household Head" },
                { "mealplans:read", "Meal Planner" },
                { "mealplans:read", "Pantry Manager" },
                { "mealplans:read", "Power Member" },
                { "mealplans:update", "Household Head" },
                { "mealplans:update", "Meal Planner" },
                { "mealplans:update", "Power Member" },
                { "members:delete", "Household Head" },
                { "members:update", "Household Head" },
                { "permissions:read", "Administrator" },
                { "recipes:manage", "Administrator" },
                { "roles:manage", "Administrator" },
                { "roles:read", "Administrator" },
                { "storages:create", "Errand Runner" },
                { "storages:create", "Grocery Shopper" },
                { "storages:create", "Household Head" },
                { "storages:create", "Meal Planner" },
                { "storages:create", "Pantry Manager" },
                { "storages:create", "Power Member" },
                { "storages:delete", "Household Head" },
                { "storages:delete", "Pantry Manager" },
                { "storages:delete", "Power Member" },
                { "storages:read", "Errand Runner" },
                { "storages:read", "Grocery Shopper" },
                { "storages:read", "Household Head" },
                { "storages:read", "Meal Planner" },
                { "storages:read", "Pantry Manager" },
                { "storages:read", "Power Member" },
                { "storages:update", "Household Head" },
                { "storages:update", "Pantry Manager" },
                { "storages:update", "Power Member" },
                { "users:delete", "Administrator" },
                { "users:manage", "Administrator" },
                { "users:read", "Administrator" }
            });

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_role_name",
            table: "role_permissions",
            column: "role_name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "role_permissions");

        migrationBuilder.DropTable(
            name: "permissions");
    }
}
