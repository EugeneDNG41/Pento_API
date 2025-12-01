namespace Pento.Domain.Activities;

public sealed class Activity
{
    public static readonly Activity CreateStorage = new Activity(
        ActivityCode.STORAGE_CREATE.ToString(),
        "Create Storage",
        "Creating a new storage location to store your food items.",
        ActivityType.Action);
    public static readonly Activity ConsumeFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_CONSUME.ToString(),
        "Consume Food Item",
        "Consuming a food item from your storage/compartment.",
        ActivityType.Action);
    public static readonly Activity CreateHousehold = new Activity(
        ActivityCode.HOUSEHOLD_CREATE.ToString(),
        "Create Household",
        "Creating a new household to manage your food, grocery lists, and meal plans with others.",
        ActivityType.Action);
    public static readonly Activity StorageQuantity = new Activity(
        StateCode.STORAGE_QUANTITY.ToString(),
        "Storage Quantity",
        "Number of storages.",
        ActivityType.State);
    public static readonly Activity StorageTypePantry = new Activity(
        StateCode.STORAGE_TYPE_PANTRY.ToString(),
        "Storage Type Pantry",
        "Number of pantry storages.",
        ActivityType.State);
    public static readonly Activity StorageTypeRefrigerator = new Activity(
        StateCode.STORAGE_TYPE_REFRIGERATOR.ToString(),
        "Storage Type Refrigerator",
        "Number of refrigerator storages.",
        ActivityType.State);
    public static readonly Activity StorageTypeFreezer = new Activity(
        StateCode.STORAGE_TYPE_FREEZER.ToString(),
        "Storage Type Freezer",
        "Number of freezer storages.",
        ActivityType.State);
    public static readonly Activity CompartmentQuantity = new Activity(
        StateCode.COMPARTMENT_QUANTITY.ToString(),
        "Compartment Quantity",
        "Number of compartments.",
        ActivityType.State);
    public static readonly Activity FoodItemQuantity = new Activity(
        StateCode.FOOD_ITEM_QUANTITY.ToString(),
        "Food Item Quantity",
        "Number of food items.",
        ActivityType.State);
    private Activity() { }
    public Activity(string code, string name, string description, ActivityType type)
    {
        Code = code;
        Name = name;
        Description = description;
        Type = type;
    }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ActivityType Type { get; private set;  }
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
    FOOD_ITEM_CONSUME,
    HOUSEHOLD_CREATE
}

public enum StateCode //IS OR HAS. How to tackle deleted items?
{
    STORAGE_QUANTITY,
    STORAGE_TYPE_PANTRY,
    STORAGE_TYPE_REFRIGERATOR,
    STORAGE_TYPE_FREEZER,
    COMPARTMENT_QUANTITY,
    FOOD_ITEM_QUANTITY
}

public enum ActivityType
{
    Action,
    State
}
