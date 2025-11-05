namespace Pento.Domain.Roles;

public sealed class Role
{
    public static readonly Role Administrator = new("Administrator", RoleType.Administrative);

    public static readonly Role HouseholdHead = new("Household Head", RoleType.Household);
    public static readonly Role PowerMember = new("Power Member", RoleType.Household);
    public static readonly Role GroceryShopper = new("Grocery Shopper", RoleType.Household);
    public static readonly Role MealPlanner = new("Meal Planner", RoleType.Household);
    public static readonly Role PantryManager = new("Pantry Manager", RoleType.Household);
    public static readonly Role ErrandRunner = new("Errand Runner", RoleType.Household);

    public static readonly Role User = new("User", RoleType.General);


    private Role(string name, RoleType type)
    {
        Name = name;
        Type = type;
    }

    private Role()
    {
    }

    public string Name { get; private set; }
    public RoleType Type { get; private set;  }
}
public enum RoleType
{
    Administrative,
    Household,
    General
}
