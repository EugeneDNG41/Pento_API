using Pento.Domain.FoodItemReservations;

namespace Pento.Application.MealPlans.Get;

public sealed record MealPlanResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    DateOnly ScheduledDate,
    string MealType,
    int Servings,
    string? Notes,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc,
    IReadOnlyList<MealPlanRecipeInfo> Recipes,
    IReadOnlyList<MealPlanFoodItemInfo> FoodItems
);
internal sealed class MealPlanFoodItemRow
{
    public Guid MealPlanId { get; set; }
    public Guid FoodItemId { get; set; }
    public Guid FoodReferenceId { get; set; }
    public Guid FoodReservationId { get; set; }

    public string FoodItemName { get; set; } = null!;
    public string FoodReferenceName { get; set; } = null!;
    public string FoodGroup { get; set; } = null!;
    public string? FoodImageUrl { get; set; }
    public decimal Quantity { get; set; }
    public string UnitAbbreviation { get; set; } = null!;
    public ReservationStatus Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsIngredientItem { get; set; }
    public Guid? RecipeId { get; set; }


}
public sealed class MealPlanRow
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string MealType { get; set; } = default!;
    public int Servings { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}


