using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Permissions;

public sealed class Permission
{
    //User general
    public static readonly Permission UserGeneral = new("user:general", "User General", "Basic user access and functionality.");

    public static readonly Permission ViewRecipes = new("recipes:read", "View Recipes", "View recipes and their details. Read-only.");
    public static readonly Permission AddRecipes = new("recipes:create", "Add Recipes", "Create new recipes.");
    public static readonly Permission UpdateRecipes = new("recipes:update", "Update Recipes", "edit your own recipes.");
    public static readonly Permission DeleteRecipes = new("recipes:delete", "Delete Recipes", "Delete your own recipes.");
    public static readonly Permission ViewGiveaways = new("giveaways:read", "View Giveaways", "View giveaway posts and details. Read-only.");

    // Household general
    public static readonly Permission ViewHousehold = new("household:read", "View Household", "View the current household’s profile, settings, and membership. Read-only.");
    public static readonly Permission ManageHousehold = new("household:update", "Manage Household", "Update household name and invite code.");
    public static readonly Permission ManageMembers = new("members:update", "Manage Members", "Change member roles within the household.");
    public static readonly Permission RemoveMembers = new("members:delete", "Remove Members", "Remove/kick members from the household and revoke their access.");

    // Storage
    public static readonly Permission ViewStorages = new("storages:read", "View Storages", "List and view all storages (pantry, fridge, etc.) and their attributes. Read-only.");
    public static readonly Permission AddStorages = new("storages:create", "Add Storages", "Create new storage locations under the household.");
    public static readonly Permission UpdateStorages = new("storages:update", "Update Storages", "Rename storages and modify their attributes.");
    public static readonly Permission DeleteStorage = new("storages:delete", "Delete Storage", "Delete a storage. Typically requires it to be empty; irreversible.");

    // Compartments
    public static readonly Permission ViewCompartments = new("compartments:read", "View Compartments", "View compartments/shelves within a storage. Read-only.");
    public static readonly Permission AddCompartments = new("compartments:create", "Add Compartments", "Create compartments within a storage (shelves, bins, drawers).");
    public static readonly Permission UpdateCompartments = new("compartments:update", "Update Compartments", "Rename/reorder compartments and edit their attributes.");
    public static readonly Permission DeleteCompartments = new("compartments:delete", "Delete Compartments", "Delete compartments. Usually blocked if they still contain items.");

    // Food items (inventory)
    public static readonly Permission ViewFoodItems = new("fooditems:read", "View Food Items", "View inventory items, quantities, and expirations. Read-only.");
    public static readonly Permission AddFoodItems = new("fooditems:create", "Add Food Items", "Add new items to inventory.");
    public static readonly Permission UpdateFoodItems = new("fooditems:update", "Update Food Items", "Edit item details and adjust quantities (consume/waste/donate).");
    public static readonly Permission DeleteFoodItems = new("fooditems:delete", "Delete Food Items", "Delete/remove items.");
    public static readonly Permission CreateGiveaways = new("giveaways:create", "Create Giveaways", "Create giveaway posts for surplus food items.");
    public static readonly Permission UpdateGiveaways = new("giveaways:update", "Update Giveaways", "Edit giveaway posts you created.");
    public static readonly Permission DeleteGiveaways = new("giveaways:delete", "Delete Giveaways", "Delete giveaway posts you created.");

    // Meal plan
    public static readonly Permission ViewMealPlans = new("mealplans:read", "View Meal Plans", "View meal plans and scheduled recipes. Read-only.");
    public static readonly Permission AddMealPlans = new("mealplans:create", "Add Meal Plans", "Create meal plans and add meals/recipes to the schedule.");
    public static readonly Permission UpdateMealPlans = new("mealplans:update", "Update Meal Plans", "Modify meal plans.");
    public static readonly Permission DeleteMealPlans = new("mealplans:delete", "Delete Meal Plans", "Delete meal plans.");

    // Grocery list
    public static readonly Permission ViewGroceries = new("groceries:read", "View Groceries", "View grocery lists and list items. Read-only.");
    public static readonly Permission AddGroceries = new("groceries:create", "Add Groceries", "Create grocery lists and add items to lists.");
    public static readonly Permission UpdateGroceries = new("groceries:update", "Update Groceries", "Edit grocery lists and list items.");
    public static readonly Permission DeleteGroceries = new("groceries:delete", "Delete Groceries", "Delete grocery lists and/or list items.");

    

    // Admin (system-wide scope)
    public static readonly Permission GetUsers = new("users:read", "View Users", "View user accounts and basic profile/usage data. Read-only.");
    public static readonly Permission ManageUsers = new("users:manage", "Manage Users", "Create/update users or change status (e.g., lock/enable). Excludes hard delete.");
    public static readonly Permission DeleteUsers = new("users:delete", "Delete Users", "Delete/deactivate user accounts according to policy.");
    public static readonly Permission ViewHouseholds = new("households:read", "View Households", "View all households across the system. Read-only.");
    public static readonly Permission ManageHouseholds = new("households:manage", "Manage Households", "Create/update/merge/archive households at the system level.");
    public static readonly Permission ViewRoles = new("roles:read", "View Roles", "View roles and their permission mappings. Read-only.");
    public static readonly Permission ManageRoles = new("roles:manage", "Manage Roles", "Create/update/delete roles and assign/unassign permissions to roles.");
    public static readonly Permission ViewPermissions = new("permissions:read", "View Permissions", "View the catalog of permissions and their descriptions. Read-only.");
    public static readonly Permission ManageGiveaways = new("giveaways:manage", "Manage Giveaways", "Create/update/approve/close giveaway posts and moderate entries.");
    public static readonly Permission ManageRecipes = new("recipes:manage", "Manage Recipes", "Create/update/delete recipes and moderate community submissions.");
    public static readonly Permission ManageFoodReferences = new("foodreferences:manage", "Manage Food References", "Manage the authoritative food reference catalog.");



    public Permission(string code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
    }
    private Permission() { }
    public string Code { get; }
    public string Name { get; }
    public string Description { get; }
}
