using NUnit.Framework;
using Pento.Domain.GroceryLists;

namespace Pento.Domain.UnitTests;

internal sealed class GroceryListTests
{
    /// <summary>
    /// Ensures constructor initializes all values correctly.
    /// </summary>
    [Test]
    public void Constructor_ValidInputs_InitializesProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        var list = new GroceryList(id, householdId, "Weekly Plan", createdBy, now);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(list.Id, Is.EqualTo(id));
            Assert.That(list.HouseholdId, Is.EqualTo(householdId));
            Assert.That(list.Name, Is.EqualTo("Weekly Plan"));
            Assert.That(list.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(list.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(list.UpdatedOnUtc, Is.EqualTo(now));
        });
    }

    /// <summary>
    /// Ensures Create() static factory maps all fields correctly.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesProperties()
    {
        // Arrange
        var householdId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        Guid userId = createdBy;
        DateTime now = DateTime.UtcNow;

        // Act
        var list = GroceryList.Create(householdId, "Groceries", createdBy, now, userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(list.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(list.HouseholdId, Is.EqualTo(householdId));
            Assert.That(list.Name, Is.EqualTo("Groceries"));
            Assert.That(list.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(list.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(list.UpdatedOnUtc, Is.EqualTo(now));
        });
    }

    /// <summary>
    /// Ensures UpdateName updates both the name and UpdatedOnUtc timestamp.
    /// </summary>
    [Test]
    public void UpdateName_UpdatesProperties()
    {
        // Arrange
        var list = GroceryList.Create(
            Guid.NewGuid(),
            "Old Name",
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        DateTime updateTime = DateTime.UtcNow.AddMinutes(1);

        // Act
        list.UpdateName("New Name", updateTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(list.Name, Is.EqualTo("New Name"));
            Assert.That(list.UpdatedOnUtc, Is.EqualTo(updateTime));
        });
    }
}
