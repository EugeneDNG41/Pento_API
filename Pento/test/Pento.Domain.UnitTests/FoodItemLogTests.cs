using NUnit.Framework;
using Pento.Domain.FoodItemLogs;

namespace Pento.Domain.UnitTests;

internal sealed class FoodItemLogTests
{
    /// <summary>
    /// Ensures constructor initializes all properties correctly.
    /// </summary>
    [Test]
    public void Constructor_ValidInputs_InitializesProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var foodItemId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        DateTime timestamp = DateTime.UtcNow;
        FoodItemLogAction action = FoodItemLogAction.Consumption;
        decimal quantity = 2.5m;
        var unitId = Guid.NewGuid();

        // Act
        var log = new FoodItemLog(
            id,
            foodItemId,
            householdId,
            userId,
            timestamp,
            action,
            quantity,
            unitId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(id));
            Assert.That(log.FoodItemId, Is.EqualTo(foodItemId));
            Assert.That(log.HouseholdId, Is.EqualTo(householdId));
            Assert.That(log.UserId, Is.EqualTo(userId));
            Assert.That(log.Timestamp, Is.EqualTo(timestamp));
            Assert.That(log.Action, Is.EqualTo(action));
            Assert.That(log.Quantity, Is.EqualTo(quantity));
            Assert.That(log.UnitId, Is.EqualTo(unitId));
        });
    }

    /// <summary>
    /// Ensures Create() factory creates a FoodItemLog
    /// with correct property mapping and generated Id.
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesLogCorrectly()
    {
        // Arrange
        var foodItemId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        DateTime timestamp = DateTime.UtcNow;
        FoodItemLogAction action = FoodItemLogAction.Discard;
        decimal quantity = 1.0m;
        var unitId = Guid.NewGuid();

        // Act
        var log = FoodItemLog.Create(
            foodItemId,
            householdId,
            userId,
            timestamp,
            action,
            quantity,
            unitId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(log.FoodItemId, Is.EqualTo(foodItemId));
            Assert.That(log.HouseholdId, Is.EqualTo(householdId));
            Assert.That(log.UserId, Is.EqualTo(userId));
            Assert.That(log.Timestamp, Is.EqualTo(timestamp));
            Assert.That(log.Action, Is.EqualTo(action));
            Assert.That(log.Quantity, Is.EqualTo(quantity));
            Assert.That(log.UnitId, Is.EqualTo(unitId));
        });
    }
}
