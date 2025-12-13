using NUnit.Framework;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;

namespace Pento.Domain.UnitTests;

internal sealed class FoodItemTests
{
    private static FoodItem CreateSample(out Guid userId)
    {
        userId = Guid.NewGuid();

        return FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Milk",
            null,
            10m,
            Guid.NewGuid(),
            new DateOnly(2025, 12, 31),
            "note",
            userId);
    }

    /// <summary>
    /// Create() initializes properties and raises FoodItemAddedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_RaisesAddedEvent()
    {
        var userId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        var item = FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Apple",
            null,
            5m,
            unitId,
            new DateOnly(2025, 1, 1),
            null,
            userId);

        Assert.Multiple(() =>
        {
            Assert.That(item.Quantity, Is.EqualTo(5m));
            Assert.That(item.UnitId, Is.EqualTo(unitId));
            Assert.That(item.AddedBy, Is.EqualTo(userId));

            FoodItemAddedDomainEvent ev = item.GetDomainEvents()
                .OfType<FoodItemAddedDomainEvent>()
                .Single();

            Assert.That(ev.FoodItemId, Is.EqualTo(item.Id));
            Assert.That(ev.Quantity, Is.EqualTo(5m));
            Assert.That(ev.UnitId, Is.EqualTo(unitId));
            Assert.That(ev.UserId, Is.EqualTo(userId));
        });
    }

  
   
    /// <summary>
    /// Consume decreases quantity and raises FoodItemConsumedDomainEvent.
    /// </summary>
    [Test]
    public void Consume_DecreasesQuantity_RaisesEvent()
    {
        FoodItem item = CreateSample(out _);

        var userId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        item.Consume(2m, 2m, unitId, userId);

        Assert.Multiple(() =>
        {
            Assert.That(item.Quantity, Is.EqualTo(8m));
            Assert.That(item.LastModifiedBy, Is.EqualTo(userId));

            FoodItemConsumedDomainEvent ev = item.GetDomainEvents()
                .OfType<FoodItemConsumedDomainEvent>()
                .Single();

            Assert.That(ev.Quantity, Is.EqualTo(2m));
            Assert.That(ev.UnitId, Is.EqualTo(unitId));
        });
    }

    /// <summary>
    /// Discard decreases quantity and raises FoodItemDiscardedDomainEvent.
    /// </summary>
    [Test]
    public void Discard_DecreasesQuantity_RaisesEvent()
    {
        FoodItem item = CreateSample(out _);

        var userId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        item.Discard(3m, 3m, unitId, userId);

        Assert.Multiple(() =>
        {
            Assert.That(item.Quantity, Is.EqualTo(7m));
            Assert.That(item.LastModifiedBy, Is.EqualTo(userId));

            Assert.That(
                item.GetDomainEvents().OfType<FoodItemDiscardedDomainEvent>().Any(),
                Is.True);
        });
    }

    /// <summary>
    /// Reserve decreases quantity and raises FoodItemReservedDomainEvent.
    /// </summary>
    [Test]
    public void Reserve_DecreasesQuantity_RaisesEvent()
    {
        FoodItem item = CreateSample(out _);

        var userId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        item.Reserve(4m, 4m, unitId, userId);

        Assert.Multiple(() =>
        {
            Assert.That(item.Quantity, Is.EqualTo(6m));
            Assert.That(item.LastModifiedBy, Is.EqualTo(userId));

            Assert.That(
                item.GetDomainEvents().OfType<FoodItemReservedDomainEvent>().Any(),
                Is.True);
        });
    }

    /// <summary>
    /// CancelReservation increases quantity and raises event.
    /// </summary>
    [Test]
    public void CancelReservation_IncreasesQuantity_RaisesEvent()
    {
        FoodItem item = CreateSample(out _);

        var reservationId = Guid.NewGuid();

        item.CancelReservation(3m, reservationId);

        Assert.Multiple(() =>
        {
            Assert.That(item.Quantity, Is.EqualTo(13m));

            FoodItemReservationCancelledDomainEvent ev = item.GetDomainEvents()
                .OfType<FoodItemReservationCancelledDomainEvent>()
                .Single();

            Assert.That(ev.ReservationId, Is.EqualTo(reservationId));
        });
    }

    /// <summary>
    /// Methods that only mutate state should update LastModifiedBy.
    /// </summary>
    [Test]
    public void StateOnlyMethods_UpdateLastModifiedBy()
    {
        FoodItem item = CreateSample(out _);
        var userId = Guid.NewGuid();

        item.ChangeUnit(Guid.NewGuid(), 20m, userId);
        Assert.That(item.LastModifiedBy, Is.EqualTo(userId));

        item.Rename("New Name", userId);
        Assert.That(item.Name, Is.EqualTo("New Name"));

        item.UpdateNotes("Updated", userId);
        Assert.That(item.Notes, Is.EqualTo("Updated"));

        item.MoveToCompartment(Guid.NewGuid(), userId);
        Assert.That(item.LastModifiedBy, Is.EqualTo(userId));
    }

    /// <summary>
    /// Trade raises traded-away event and returns a new FoodItem
    /// with traded-in event.
    /// </summary>
    [Test]
    public void Trade_RaisesBothTradeEvents()
    {
        FoodItem item = CreateSample(out Guid ownerId);

        var tradeUserId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        var newCompartment = Guid.NewGuid();

        FoodItem tradedItem = item.Trade(
            Guid.NewGuid(),
            tradeUserId,
            newCompartment,
            2m,
            unitId,
            ownerId);

        Assert.Multiple(() =>
        {
            Assert.That(
                item.GetDomainEvents().OfType<FoodItemTradedAwayDomainEvent>().Any(),
                Is.True);

            Assert.That(
                tradedItem.GetDomainEvents().OfType<FoodItemTradedInDomainEvent>().Any(),
                Is.True);

            Assert.That(tradedItem.Quantity, Is.EqualTo(2m));
            Assert.That(tradedItem.CompartmentId, Is.EqualTo(newCompartment));
        });
    }
}
