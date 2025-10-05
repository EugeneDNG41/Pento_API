using System.ComponentModel.DataAnnotations;

namespace Pento.Domain.Users;

public sealed class Permission
{
    public static readonly Permission GetUser = new("users:read");
    public static readonly Permission ModifyUser = new("users:update");
    // Household general
    public static readonly Permission ViewHousehold = new("household:view");
    public static readonly Permission UpdateHousehold = new("household:update");
    public static readonly Permission TransferOwner = new("household:transferOwner");

    // Member management
    public static readonly Permission ViewMembers = new("members:read");
    public static readonly Permission InviteMember = new("members:invite");
    public static readonly Permission UpdateMember = new("members:update");
    public static readonly Permission RemoveMember = new("members:remove");

    // Storage / Pantry
    public static readonly Permission ViewStorage = new("storage:read");
    public static readonly Permission ManageStorage = new("storage:manage");

    public static readonly Permission ViewPantry = new("pantry:read");
    public static readonly Permission AddPantryItem = new("pantry:create");
    public static readonly Permission UpdatePantry = new("pantry:update");
    public static readonly Permission ConsumePantry = new("pantry:consume");
    public static readonly Permission DiscardPantry = new("pantry:discard");

    // Grocery list
    public static readonly Permission ViewGroceries = new("grocery:read");
    public static readonly Permission ManageGroceries = new("grocery:manage");

    // Meal plan
    public static readonly Permission ViewMealPlans = new("mealplan:read");
    public static readonly Permission ManageMealPlans = new("mealplan:manage");

    // Recipes
    public static readonly Permission ViewRecipes = new("recipes:read");
    public static readonly Permission ManageRecipes = new("recipes:manage");

    // Giveaways
    public static readonly Permission ViewGiveaways = new("giveaways:read");
    public static readonly Permission ManageGiveaways = new("giveaways:manage");

    // Admin / elevated
    public static readonly Permission ManagePermissions = new("permissions:manage");
    public static readonly Permission ManageOverrides = new("permissions:overrides");


    public Permission(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
