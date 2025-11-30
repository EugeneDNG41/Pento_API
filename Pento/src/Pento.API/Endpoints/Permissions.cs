using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Permissions;

namespace Pento.API.Endpoints;
internal sealed class Permissions
{

    //User general
    internal const string UserGeneral = "user:general";
    internal const string ViewRecipes = "recipes:read";
    internal const string AddRecipes = "recipes:create";
    internal const string UpdateRecipes = "recipes:update";
    internal const string DeleteRecipes = "recipes:delete";
    internal const string ViewGiveaways = "giveaways:read";

    // Household general
    internal const string ViewHousehold = "household:read";
    internal const string ManageHousehold = "household:update";
    internal const string ManageMembers = "members:update";
    internal const string RemoveMembers = "members:delete";

    // Storage
    internal const string ViewStorages = "storages:read";
    internal const string AddStorages = "storages:create";
    internal const string UpdateStorages = "storages:update";
    internal const string DeleteStorage = "storages:delete";

    internal const string ViewCompartments = "compartments:read";
    internal const string AddCompartments = "compartments:create";
    internal const string UpdateCompartments = "compartments:update";
    internal const string DeleteCompartments = "compartments:delete";

    internal const string ViewFoodItems = "fooditems:read";
    internal const string AddFoodItems = "fooditems:create";
    internal const string UpdateFoodItems = "fooditems:update";
    internal const string DeleteFoodItems = "fooditems:delete";
    internal const string CreateGiveaways = "giveaways:create";
    internal const string UpdateGiveaways = "giveaways:update";
    internal const string DeleteGiveaways = "giveaways:delete";

    // Grocery list
    internal const string ViewGroceries = "groceries:read";
    internal const string AddGroceries = "groceries:create";
    internal const string UpdateGroceries = "groceries:update";
    internal const string DeleteGroceries = "groceries:delete";

    // Meal plan
    internal const string ViewMealPlans = "mealplans:read";
    internal const string AddMealPlans = "mealplans:creat";
    internal const string UpdateMealPlans = "mealplans:udpate";
    internal const string DeleteMealPlans = "mealplans:delete";

    // Admin
    internal const string GetUsers = "users:read";
    internal const string ManageUsers = "users:manage";
    internal const string DeleteUsers = "users:delete";
    internal const string ViewHouseholds = "households:read";
    internal const string ManageHouseholds = "households:manage";
    internal const string ViewRoles = "roles:read";
    internal const string ManageRoles = "roles:manage";
    internal const string ViewPermissions = "permissions:read";
    internal const string ManageGiveaways = "giveaways:manage";
    internal const string ManageRecipes = "recipes:manage";
    internal const string ManageFoodReferences = "foodreferences:manage";
    internal const string ManagePayments = "payments:manage";
    internal const string ManageSubscriptions = "subscriptions:manage";
    internal const string ManageMilestones = "milestones:manage";
}
