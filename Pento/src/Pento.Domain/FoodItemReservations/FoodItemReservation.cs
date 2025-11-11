using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodItemReservations;

public abstract class FoodItemReservation : Entity
{
    public Guid FoodItemId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public DateTime ReservationDateUtc { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public ReservationFor ReservationFor { get; private set; }

}
public enum ReservationStatus
{
    Pending,
    Fulfilled,
    Cancelled
}
public enum ReservationFor
{
    Recipe,
    MealPlan,
    Donation
}
public sealed class FoodItemRecipeReservation : FoodItemReservation
{
    public Guid RecipeId { get; private set; }
}
public sealed class FoodItemMealPlanReservation : FoodItemReservation
{
    public Guid MealPlanId { get; private set; }
}
public sealed class FoodItemDonationReservation : FoodItemReservation
{
    public Guid GiveawayPostId { get; private set; }
}
