using NUnit.Framework;
using Pento.Domain.RecipeIngredients;

namespace Pento.Domain.UnitTests;

internal sealed class RecipeIngredientTests
{
    /// <summary>
    /// Verifies that Create() initializes all properties correctly.
    /// No domain events are raised during creation.
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesIngredientCorrectly()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var foodRefId = Guid.NewGuid();
        decimal quantity = 2.5m;
        var unitId = Guid.NewGuid();
        string? notes = "Chopped";
        DateTime now = DateTime.UtcNow;

        // Act
        var ingredient = RecipeIngredient.Create(
            recipeId,
            foodRefId,
            quantity,
            unitId,
            notes,
            now);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ingredient.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(ingredient.RecipeId, Is.EqualTo(recipeId));
            Assert.That(ingredient.FoodRefId, Is.EqualTo(foodRefId));
            Assert.That(ingredient.Quantity, Is.EqualTo(quantity));
            Assert.That(ingredient.UnitId, Is.EqualTo(unitId));
            Assert.That(ingredient.Notes, Is.EqualTo(notes));
            Assert.That(ingredient.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(ingredient.UpdatedOnUtc, Is.EqualTo(now));

            // No events are raised
            Assert.That(ingredient.GetDomainEvents().Count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Verifies that UpdateDetails updates quantity, unitId, notes,
    /// and timestamp when valid values are provided.
    /// </summary>
    [Test]
    public void UpdateDetails_ValidInputs_UpdatesPropertiesCorrectly()
    {
        // Arrange
        var ingredient = RecipeIngredient.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1.0m,
            Guid.NewGuid(),
            "Old notes",
            DateTime.UtcNow);

        decimal newQuantity = 5.0m;
        var newUnitId = Guid.NewGuid();
        string newNotes = "Fresh chopped";
        DateTime newUtc = DateTime.UtcNow.AddMinutes(10);

        // Act
        ingredient.UpdateDetails(newQuantity, newUnitId, newNotes, newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ingredient.Quantity, Is.EqualTo(newQuantity));
            Assert.That(ingredient.UnitId, Is.EqualTo(newUnitId));
            Assert.That(ingredient.Notes, Is.EqualTo(newNotes));
            Assert.That(ingredient.UpdatedOnUtc, Is.EqualTo(newUtc));

            Assert.That(ingredient.GetDomainEvents().Count, Is.EqualTo(0)); // No event raised
        });
    }

    /// <summary>
    /// Verifies that UpdateDetails does NOT overwrite quantity
    /// when quantity <= 0 or null.
    /// </summary>
    [TestCase(null)]
    [TestCase(0)]
    [TestCase(-5)]
    public void UpdateDetails_InvalidQuantity_DoesNotChangeQuantity(decimal? invalidQuantity)
    {
        // Arrange
        var ingredient = RecipeIngredient.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            3.0m,
            Guid.NewGuid(),
            "Initial",
            DateTime.UtcNow);

        decimal originalQuantity = ingredient.Quantity;

        DateTime newUtc = DateTime.UtcNow.AddMinutes(5);

        // Act
        ingredient.UpdateDetails(invalidQuantity, null, "Updated", newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ingredient.Quantity, Is.EqualTo(originalQuantity), "Quantity should remain unchanged for invalid input");
            Assert.That(ingredient.Notes, Is.EqualTo("Updated"));
            Assert.That(ingredient.UpdatedOnUtc, Is.EqualTo(newUtc));
            Assert.That(ingredient.GetDomainEvents().Count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Verifies that UpdateDetails does NOT modify UnitId when null is provided.
    /// </summary>
    [Test]
    public void UpdateDetails_UnitIdNull_DoesNotChangeUnitId()
    {
        // Arrange
        var ingredient = RecipeIngredient.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1.0m,
            Guid.NewGuid(),
            "Initial",
            DateTime.UtcNow);

        Guid originalUnitId = ingredient.UnitId;
        DateTime newUtc = DateTime.UtcNow.AddMinutes(3);

        // Act
        ingredient.UpdateDetails(10m, null, "Updated", newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ingredient.UnitId, Is.EqualTo(originalUnitId));
            Assert.That(ingredient.Notes, Is.EqualTo("Updated"));
            Assert.That(ingredient.UpdatedOnUtc, Is.EqualTo(newUtc));
            Assert.That(ingredient.GetDomainEvents().Count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Verifies that Notes is always updated
    /// even if quantity or unitId are not changed.
    /// </summary>
    [Test]
    public void UpdateDetails_AlwaysUpdatesNotes()
    {
        // Arrange
        var ingredient = RecipeIngredient.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            2.0m,
            Guid.NewGuid(),
            "Original",
            DateTime.UtcNow);

        DateTime newUtc = DateTime.UtcNow.AddMinutes(7);

        // Act
        ingredient.UpdateDetails(null, null, "New Notes", newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ingredient.Notes, Is.EqualTo("New Notes"));
            Assert.That(ingredient.UpdatedOnUtc, Is.EqualTo(newUtc));
            Assert.That(ingredient.GetDomainEvents().Count, Is.EqualTo(0));
        });
    }
}
