using NUnit.Framework;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Units;

namespace Pento.Domain.UnitTests;

internal sealed class FoodItemReservationTests
{
    /// <summary>
    /// Verifies that Create() initializes values correctly for FoodItemRecipeReservation.
    /// </summary>
    [Test]
    public void Create_RecipeReservation_InitializesCorrectly()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var hhId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        var res = FoodItemRecipeReservation.Create(
            foodId, hhId, now, 5m, unitId, recipeId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(res.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(res.FoodItemId, Is.EqualTo(foodId));
            Assert.That(res.HouseholdId, Is.EqualTo(hhId));
            Assert.That(res.ReservationDateUtc, Is.EqualTo(now));
            Assert.That(res.Quantity, Is.EqualTo(5m));
            Assert.That(res.UnitId, Is.EqualTo(unitId));
            Assert.That(res.RecipeId, Is.EqualTo(recipeId));
            Assert.That(res.Status, Is.EqualTo(ReservationStatus.Pending));
            Assert.That(res.ReservationFor, Is.EqualTo(ReservationFor.Recipe));
        });
    }

    /// <summary>
    /// Verifies UpdateQuantity changes quantity only.
    /// </summary>
    [Test]
    public void UpdateQuantity_SetsQuantity()
    {
        var res = FoodItemMealPlanReservation.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
            3m, Guid.NewGuid(), Guid.NewGuid());

        res.UpdateQuantity(10m);

        Assert.That(res.Quantity, Is.EqualTo(10m));
    }

    /// <summary>
    /// Verifies UpdateUnit sets the new unit id.
    /// </summary>
    [Test]
    public void UpdateUnit_SetsUnitId()
    {
        var res = FoodItemMealPlanReservation.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
            2m, Guid.NewGuid(), Guid.NewGuid());

        var newUnit = Guid.NewGuid();

        res.UpdateUnit(newUnit);

        Assert.That(res.UnitId, Is.EqualTo(newUnit));
    }

    /// <summary>
    /// Verifies Increase() increments quantity.
    /// </summary>
    [Test]
    public void Increase_AddsQuantity()
    {
        var res = FoodItemMealPlanReservation.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
            2m, Guid.NewGuid(), Guid.NewGuid());

        res.Increase(3m);

        Assert.That(res.Quantity, Is.EqualTo(5m));
    }

    /// <summary>
    /// Verifies MarkAsFulfilled updates fields, sets status,
    /// and raises FoodItemConsumedDomainEvent.
    /// </summary>
    [Test]
    public void MarkAsFulfilled_Pending_RaisesEventAndUpdatesFields()
    {
        // Arrange
        var res = FoodItemRecipeReservation.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
            5m, Guid.NewGuid(), Guid.NewGuid());


        var userId = Guid.NewGuid();
        var newUnit = Guid.NewGuid();

        // Act
        res.MarkAsFulfilled(10m, newUnit, userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(res.Quantity, Is.EqualTo(10m));
            Assert.That(res.UnitId, Is.EqualTo(newUnit));
            Assert.That(res.Status, Is.EqualTo(ReservationStatus.Fulfilled));


            FoodItemConsumedDomainEvent ev = res.GetDomainEvents().OfType<FoodItemConsumedDomainEvent>().Single();
            Assert.That(ev.FoodItemId, Is.EqualTo(res.FoodItemId));
            Assert.That(ev.Quantity, Is.EqualTo(10m));
            Assert.That(ev.UnitId, Is.EqualTo(newUnit));
            Assert.That(ev.UserId, Is.EqualTo(userId));
        });
    }

    /// <summary>
    /// Verifies MarkAsFulfilled throws if reservation is not pending.
    /// </summary>
    [Test]
    public void MarkAsFulfilled_NotPending_Throws()
    {
        var res = FoodItemMealPlanReservation.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
            2m, Guid.NewGuid(), Guid.NewGuid());

        res.MarkAsCancelled(Guid.NewGuid()); // status = Cancelled

        Assert.Throws<Exception>(() =>
            res.MarkAsFulfilled(1m, Guid.NewGuid(), Guid.NewGuid()));
    }

    /// <summary>
    /// Verifies MarkAsCancelled works only when Pending.
    /// </summary>
    [Test]
    public void MarkAsCancelled_Pending_SetsCancelled()
    {
        var res = FoodItemRecipeReservation.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
            3m, Guid.NewGuid(), Guid.NewGuid());

        res.MarkAsCancelled(Guid.NewGuid());

        Assert.That(res.Status, Is.EqualTo(ReservationStatus.Cancelled));
    }

    [Test]
    public void MarkAsCancelled_NotPending_Throws()
    {
        var res = new FoodItemTradeReservation(
           Guid.CreateVersion7(),
           Guid.NewGuid(),
           Guid.NewGuid(),
           DateTime.UtcNow,
           2m,
           Guid.NewGuid(),
           ReservationStatus.Fulfilled,
           ReservationFor.Trade,
           Guid.NewGuid());

        Assert.Throws<Exception>(() => res.MarkAsCancelled(Guid.NewGuid()));
    }


}
