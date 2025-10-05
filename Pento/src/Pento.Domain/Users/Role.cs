namespace Pento.Domain.Users;

public sealed class Role
{
    public static readonly Role Administrator = new("Administrator");
    public static readonly Role HouseholdAdmin = new("Household Admin");

    public static readonly Role PowerMember = new("Power Member");
    public static readonly Role GroceryRunner = new("Grocery Runner");
    public static readonly Role MealPlanner = new("Meal Planner");
    public static readonly Role StorageManager = new("Storage Manager");
    public static readonly Role BasicMember = new("Basic Member");


    private Role(string name)
    {
        Name = name;
    }

    private Role()
    {
    }

    public string Name { get; private set; }
}
