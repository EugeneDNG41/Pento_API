namespace Pento.Domain.Activities;

public sealed class Activity
{
    public static readonly Activity CreateStorage = new Activity(
        ActivityCode.STORAGE_CREATE.ToString(),
        "Create Storage",
        "Creating a new storage location to store your food items.");
    public static readonly Activity CreateCompartment = new Activity(
        ActivityCode.COMPARTMENT_CREATE.ToString(),
        "Create Compartment",
        "Creating a new compartment within a storage location to better organize your food items.");
    public static readonly Activity ConsumeFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_CONSUME.ToString(),
        "Consume Food Item",
        "Consuming a food item from your storage/compartment.");
    public static readonly Activity IntakeFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_INTAKE.ToString(),
        "Intake Food Item",
        "Adding a new food item to your storage/compartment.");
    public static readonly Activity DiscardFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_DISCARD.ToString(),
        "Discard Food Item",
        "Discarding a food item from your storage/compartment.");
    public static readonly Activity CreateRecipe = new Activity(
        ActivityCode.RECIPE_CREATE.ToString(),
        "Create Recipe",
        "Creating a new recipe to plan your meals and manage ingredients.");
    public static readonly Activity CookRecipe = new Activity(
        ActivityCode.RECIPE_COOK.ToString(),
        "Cook Recipe",
        "Cooking a recipe using ingredients from your food items.");
    public static readonly Activity CookOtherRecipe = new Activity(
        ActivityCode.RECIPE_OTHER_COOK.ToString(),
        "Recipe Cooked By Other",
        "Having your recipe cooked by another user");
    public static readonly Activity CreateMealPlan = new Activity(
        ActivityCode.MEAL_PLAN_CREATE.ToString(),
        "Create Meal Plan",
        "Creating a new meal plan to organize your meals for the week.");
    public static readonly Activity FulfillMealPlan = new Activity(
        ActivityCode.MEAL_PLAN_FULFILLED.ToString(),
        "Fulfill Meal Plan",
        "Completing a meal plan by preparing the planned meals.");
    public static readonly Activity CancelMealPlan = new Activity(
        ActivityCode.MEAL_PLAN_CANCELLED.ToString(),
        "Cancel Meal Plan",
        "Cancelling a meal plan that is no longer needed.");
    public static readonly Activity CreateGroceryList = new Activity(
        ActivityCode.GROCERY_LIST_CREATE.ToString(),
        "Create Grocery List",
        "Creating a new grocery list to keep track of items you need to buy.");
    public static readonly Activity CreateHousehold = new Activity(
        ActivityCode.HOUSEHOLD_CREATE.ToString(),
        "Create Household",
        "Creating a new household to manage your food, grocery lists, and meal plans with others.");
    public static readonly Activity JoinHousehold = new Activity(
        ActivityCode.HOUSEHOLD_JOIN.ToString(),
        "Join Household",
        "Joining an existing household to collaborate on food management.");
    public static readonly Activity HouseholdMemberJoined = new Activity(
        ActivityCode.HOUSEHOLD_MEMBER_JOINED.ToString(),
        "Household Member Joined",
        "A new member has joined your household to help manage food, grocery lists, and meal plans.");
    public static readonly Activity TradeInFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_TRADE_IN.ToString(),
        "Trade In Food Item",
        "Receiving a food item from another household through trade.");
    public static readonly Activity TradeAwayFoodItem = new Activity(
        ActivityCode.FOOD_ITEM_TRADE_AWAY.ToString(),
        "Trade Away Food Item",
        "Giving a food item to another household through trade.");
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
    RECIPE_CREATE,
    RECIPE_COOK,
    RECIPE_OTHER_COOK,
    MEAL_PLAN_CREATE,
    MEAL_PLAN_FULFILLED,
    MEAL_PLAN_CANCELLED,
    GROCERY_LIST_CREATE,
    HOUSEHOLD_CREATE,
    HOUSEHOLD_JOIN,
    HOUSEHOLD_MEMBER_JOINED,
    FOOD_ITEM_TRADE_IN,
    FOOD_ITEM_TRADE_AWAY

}
