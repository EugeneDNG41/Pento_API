using NUnit.Framework;
using Pento.Domain.GroceryListItems;

namespace Pento.Domain.UnitTests;

internal sealed class GroceryListItemTests
{
    /// <summary>
    /// Ensures constructor initializes all fields properly.
    /// </summary>
    [Test]
    public void Constructor_ValidInputs_InitializesProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var foodRefId = Guid.NewGuid();
        var addedBy = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        var item = new GroceryListItem(
            id,
            listId,
            foodRefId,
            3.5m,
            addedBy,
            now,
            customName: "Tomatoes",
            unitId: unitId,
            notes: "Roma only",
            priority: GroceryItemPriority.High
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(item.Id, Is.EqualTo(id));
            Assert.That(item.ListId, Is.EqualTo(listId));
            Assert.That(item.FoodRefId, Is.EqualTo(foodRefId));
            Assert.That(item.Quantity, Is.EqualTo(3.5m));
            Assert.That(item.AddedBy, Is.EqualTo(addedBy));
            Assert.That(item.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(item.CustomName, Is.EqualTo("Tomatoes"));
            Assert.That(item.UnitId, Is.EqualTo(unitId));
            Assert.That(item.Notes, Is.EqualTo("Roma only"));
            Assert.That(item.Priority, Is.EqualTo(GroceryItemPriority.High));
        });
    }

    /// <summary>
    /// Verifies Update() overwrites editable fields.
    /// </summary>
    [Test]
    public void Update_ValidInputs_UpdatesAllEditableFields()
    {
        // Arrange
        var item = new GroceryListItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2m,
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        // Act
        item.Update(
            quantity: 10m,
            notes: "fresh only",
            customName: "Apples",
            priority: GroceryItemPriority.Low
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(item.Quantity, Is.EqualTo(10m));
            Assert.That(item.Notes, Is.EqualTo("fresh only"));
            Assert.That(item.CustomName, Is.EqualTo("Apples"));
            Assert.That(item.Priority, Is.EqualTo(GroceryItemPriority.Low));
        });
    }

    /// <summary>
    /// Verifies IncreaseQuantity() adds correctly when amount > 0.
    /// </summary>
    [Test]
    public void IncreaseQuantity_PositiveAmount_IncreasesQuantity()
    {
        // Arrange
        var item = new GroceryListItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5m,
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        // Act
        item.IncreaseQuantity(3m);

        // Assert
        Assert.That(item.Quantity, Is.EqualTo(8m));
    }

    /// <summary>
    /// Verifies IncreaseQuantity() ignores zero or negative amounts.
    /// </summary>
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-5)]
    public void IncreaseQuantity_NonPositiveAmount_DoesNotChangeQuantity(decimal amount)
    {
        // Arrange
        var item = new GroceryListItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            4m,
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        // Act
        item.IncreaseQuantity(amount);

        // Assert
        Assert.That(item.Quantity, Is.EqualTo(4m));
    }
}
