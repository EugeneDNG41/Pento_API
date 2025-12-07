using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;

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
    public void MarkAsCancelled()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new("Only pending reservations can be cancelled.");
        }

        Status = ReservationStatus.Cancelled;
    }
    public void Increase(decimal additionalQuantity)
    {
        Quantity += additionalQuantity;
    }
    public (FoodItemReservation? fulfilledReservation, decimal remainingQuantity)
    FulfillPartially(decimal fulfillQuantity, Guid unitId, Guid userId)
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new("Only pending reservations can be fulfilled.");
        }

        if (fulfillQuantity <= 0)
        {
            throw new("Fulfilled quantity must be greater than zero.");
        }

        if (fulfillQuantity > Quantity)
        {
            throw new("Cannot fulfill more than reserved quantity.");
        }

        if (fulfillQuantity == Quantity)
        {
            MarkAsFulfilled(fulfillQuantity, unitId, userId);
            return (this, 0);
        }

        Quantity -= fulfillQuantity;

        FoodItemReservation fulfilled = CloneAsFulfilled(fulfillQuantity, unitId, userId);

        fulfilled.Raise(new FoodItemConsumedDomainEvent(
            FoodItemId,
            fulfillQuantity,
            unitId,
            userId));

        return (fulfilled, Quantity);
    }
    protected abstract FoodItemReservation CloneAsFulfilled(
    decimal quantity,
    Guid unitId,
    Guid userId);

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
    Trade
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
    protected override FoodItemReservation CloneAsFulfilled(
    decimal quantity,
    Guid unitId,
    Guid userId)
    {
        return new FoodItemRecipeReservation(
            Guid.CreateVersion7(),
            FoodItemId,
            HouseholdId,
            DateTime.UtcNow,
            quantity,
            unitId,
            ReservationStatus.Fulfilled,
            ReservationFor.Recipe,
            RecipeId
        );
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
    protected override FoodItemReservation CloneAsFulfilled(
    decimal quantity,
    Guid unitId,
    Guid userId)
    {
        var r = new FoodItemMealPlanReservation(
            Guid.CreateVersion7(),
            FoodItemId,
            HouseholdId,
            DateTime.UtcNow,
            quantity,
            unitId,
            ReservationStatus.Fulfilled,
            ReservationFor.MealPlan,
            MealPlanId
        );

        return r;
    }



}
public sealed class FoodItemTradeReservation : FoodItemReservation
{
    public Guid TradeItemId { get; private set; }

    private FoodItemTradeReservation() { }

    public FoodItemTradeReservation(
        Guid id,
        Guid foodItemId,
        Guid householdId,
        DateTime reservationDateUtc,
        decimal quantity,
        Guid unitId,
        ReservationStatus reservationStatus,
        ReservationFor reservationFor,
        Guid tradeItemId
    ) : base(id, foodItemId, householdId, reservationDateUtc, quantity, unitId, reservationStatus, reservationFor)
    {
        TradeItemId = tradeItemId;
    }

    protected override FoodItemReservation CloneAsFulfilled(
        decimal quantity,
        Guid unitId,
        Guid userId)
    {
        return new FoodItemTradeReservation(
            Guid.CreateVersion7(),
            FoodItemId,
            HouseholdId,
            DateTime.UtcNow,
            quantity,
            unitId,
            ReservationStatus.Fulfilled,
            ReservationFor.Trade, 
            TradeItemId
        );
    }
}



