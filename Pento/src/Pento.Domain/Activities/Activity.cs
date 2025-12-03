namespace Pento.Domain.Activities;

public sealed class Activity
{
    public static readonly Activity CreateStorage = new Activity(
        ActivityCode.STORAGE_CREATE.ToString(),
        "Create Storage",
        "Creating a new storage location to store your food items.");
    public static readonly Activity ConsumeFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_CONSUME.ToString(),
        "Consume Food Item",
        "Consuming a food item from your storage/compartment.");
    public static readonly Activity CreateHousehold = new Activity(
        ActivityCode.HOUSEHOLD_CREATE.ToString(),
        "Create Household",
        "Creating a new household to manage your food, grocery lists, and meal plans with others.");
    private Activity() { }
    public Activity(string code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
    }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public void UpdateDetails(string? name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
    }
}
public enum ActivityCode //DO OR PERFORM
{
    STORAGE_CREATE,
    COMPARTMENT_CREATE,
    FOOD_ITEM_CONSUME,
    FOOD_ITEM_INTAKE,
    FOOD_ITEM_DISCARD,
    RESERVATION_CREATE,
    RESERVATION_FULFILLED,
    RESERVATION_CANCELLED,
    RECIPE_CREATE,
    MEAL_PLAN_CREATE,
    GROCERY_LIST_CREATE,
    HOUSEHOLD_CREATE,
    HOUSEHOLD_JOIN,
    HOUSEHOLD_MEMBER_JOINED,

}
