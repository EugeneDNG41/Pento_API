using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

namespace Pento.Domain.FoodItemReservations;

public abstract class FoodItemReservation : Entity
{
    protected FoodItemReservation(Guid id, 
        Guid foodItemId, 
        Guid householdId, 
        DateTime reservationDateUtc, 
        decimal quantity, 
        Guid unitId, 
        ReservationStatus status, 
        ReservationFor reservationFor) : base(id)
    {
        FoodItemId = foodItemId;
        HouseholdId = householdId;
        ReservationDateUtc = reservationDateUtc;
        Quantity = quantity;
        UnitId = unitId;
        Status = status;
        ReservationFor = reservationFor;
    }
    protected FoodItemReservation() { }

    public Guid FoodItemId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public DateTime ReservationDateUtc { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public ReservationStatus Status { get; protected set; } 
    public ReservationFor ReservationFor { get; private set; }
    public void UpdateQuantity(decimal newQuantity)
    {

        Quantity = newQuantity;
    }
    public void UpdateUnit(Guid newUnitId)
    {
        UnitId = newUnitId;
    }
    public void MarkAsFulfilled(decimal quantity, Guid unitId, Guid userId)
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new("Only pending reservations can be fulfilled.");
        }
        Quantity = quantity;
        UnitId = unitId;
        Status = ReservationStatus.Fulfilled;
        Raise(new FoodItemConsumedDomainEvent(FoodItemId, quantity, unitId, userId));
    }

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
    private FoodItemRecipeReservation() { }
    public Guid RecipeId { get; private set; }
    public FoodItemRecipeReservation(
        Guid id,
        Guid foodItemId,
        Guid householdId,
        DateTime reservationDateUtc,
        decimal quantity,
        Guid unitId,
        ReservationStatus reservationStatus,
        ReservationFor reservationFor,
        Guid recipeId
        ) : base(id, foodItemId, householdId, reservationDateUtc, quantity, unitId, reservationStatus, reservationFor)
    {
        RecipeId = recipeId;
    }
    public static FoodItemRecipeReservation Create(
        Guid foodItemId,
        Guid householdId,
        DateTime reservationDateUtc,
        decimal quantity,
        Guid unitId,
        Guid recipeId
        )
    {
        var reciperservation = new FoodItemRecipeReservation(
            Guid.CreateVersion7(),
            foodItemId,
            householdId,
            reservationDateUtc,
            quantity,
            unitId,
            ReservationStatus.Pending,
            ReservationFor.Recipe,
            recipeId
            );
        return reciperservation;
    }
    
    public void MarkAsCancelled()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new ("Only pending reservations can be cancelled.");
        }

        Status = ReservationStatus.Cancelled;
    }
}
public sealed class FoodItemMealPlanReservation : FoodItemReservation
{
    public Guid MealPlanId { get; private set; }
    private FoodItemMealPlanReservation() { }

    public FoodItemMealPlanReservation(
    Guid id,
    Guid foodItemId,
    Guid householdId,
    DateTime reservationDateUtc,
    decimal quantity,
    Guid unitId,
    ReservationStatus reservationStatus,
    ReservationFor reservationFor,
    Guid mealplanId
    ) : base(id, foodItemId, householdId, reservationDateUtc, quantity, unitId, reservationStatus, reservationFor)
    {
        MealPlanId = mealplanId;
    }
    public static FoodItemMealPlanReservation Create(
      Guid foodItemId,
      Guid householdId,
      DateTime reservationDateUtc,
      decimal quantity,
      Guid unitId,
      Guid mealPlanId
  )
    {
        return new FoodItemMealPlanReservation(
            Guid.CreateVersion7(),
            foodItemId,
            householdId,
            reservationDateUtc,
            quantity,
            unitId,
            ReservationStatus.Pending,
            ReservationFor.MealPlan,
            mealPlanId
        );
    }
    public void MarkAsCancelled()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new("Only pending reservations can be cancelled.");
        }

        Status = ReservationStatus.Cancelled;
    }

}
public sealed class FoodItemDonationReservation : FoodItemReservation
{
    public Guid GiveawayPostId { get; private set; }
    private FoodItemDonationReservation() { }

    public FoodItemDonationReservation(
    Guid id,
    Guid foodItemId,
    Guid householdId,
    DateTime reservationDateUtc,
    decimal quantity,
    Guid unitId,
    ReservationStatus reservationStatus,
    ReservationFor reservationFor,
    Guid giveAwayPostId
) : base(id, foodItemId, householdId, reservationDateUtc, quantity, unitId, reservationStatus, reservationFor)
    {
        GiveawayPostId = giveAwayPostId;
    }

}

