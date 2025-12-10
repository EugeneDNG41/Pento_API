using NUnit.Framework;
using Pento.Domain.MealPlans;
using Pento.Domain.MealPlans.Events;

namespace Pento.Domain.UnitTests;

internal sealed class MealPlanTests
{
    /// <summary>
    /// Ensures constructor sets all properties properly,
    /// including fallback servings > 0 → else = 1.
    /// </summary>
    [Test]
    public void Constructor_ValidInputs_InitializesProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var date = new DateOnly(2025, 12, 10);
        DateTime now = DateTime.UtcNow;

        // Act
        var meal = new MealPlan(
            id,
            householdId,
            "Chicken Soup",
            MealType.Dinner,
            date,
            servings: 2,
            notes: "extra spicy",
            createdBy,
            now
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(meal.Id, Is.EqualTo(id));
            Assert.That(meal.HouseholdId, Is.EqualTo(householdId));
            Assert.That(meal.Name, Is.EqualTo("Chicken Soup"));
            Assert.That(meal.MealType, Is.EqualTo(MealType.Dinner));
            Assert.That(meal.ScheduledDate, Is.EqualTo(date));
            Assert.That(meal.Servings, Is.EqualTo(2));
            Assert.That(meal.Notes, Is.EqualTo("extra spicy"));
            Assert.That(meal.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(meal.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(meal.UpdatedOnUtc, Is.EqualTo(now));
        });
    }

    /// <summary>
    /// Ensures servings fallback to 1 when <= 0.
    /// </summary>
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-10)]
    public void Constructor_InvalidServings_FallbacksToOne(int invalid)
    {
        var meal = new MealPlan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Meal",
            MealType.Breakfast,
            new DateOnly(2025, 1, 1),
            invalid,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        Assert.That(meal.Servings, Is.EqualTo(1));
    }

    /// <summary>
    /// Ensures Create() maps values and raises MealPlanCreatedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_RaisesCreatedEventAndMapsValues()
    {
        // Arrange
        var householdId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var date = new DateOnly(2025, 12, 8);
        DateTime now = DateTime.UtcNow;

        // Act
        var meal = MealPlan.Create(
            householdId,
            "Curry Rice",
            MealType.Lunch,
            date,
            3,
            "mild",
            createdBy,
            now
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(meal.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(meal.HouseholdId, Is.EqualTo(householdId));
            Assert.That(meal.Name, Is.EqualTo("Curry Rice"));
            Assert.That(meal.MealType, Is.EqualTo(MealType.Lunch));
            Assert.That(meal.ScheduledDate, Is.EqualTo(date));
            Assert.That(meal.Servings, Is.EqualTo(3));
            Assert.That(meal.Notes, Is.EqualTo("mild"));
            Assert.That(meal.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(meal.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(meal.UpdatedOnUtc, Is.EqualTo(now));

            // verify event
            MealPlanCreatedDomainEvent ev = meal.GetDomainEvents().OfType<MealPlanCreatedDomainEvent>().Single();
            Assert.That(ev.MealPlanId, Is.EqualTo(meal.Id));
            Assert.That(ev.UserId, Is.EqualTo(createdBy));
        });
    }

    /// <summary>
    /// Ensures Update() changes fields, updates timestamp,
    /// and raises MealPlanUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void Update_ValidInputs_UpdatesFieldsAndRaisesEvent()
    {
        // Arrange
        var meal = MealPlan.Create(
            Guid.NewGuid(),
            "Old Name",
            MealType.Breakfast,
            new DateOnly(2025, 1, 1),
            2,
            "old notes",
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        DateTime updatedAt = DateTime.UtcNow.AddHours(1);

        // Act
        meal.Update(
            MealType.Dinner,
            new DateOnly(2025, 2, 2),
            servings: 10,
            notes: "updated notes",
            updatedAt
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(meal.MealType, Is.EqualTo(MealType.Dinner));
            Assert.That(meal.ScheduledDate, Is.EqualTo(new DateOnly(2025, 2, 2)));
            Assert.That(meal.Servings, Is.EqualTo(10));
            Assert.That(meal.Notes, Is.EqualTo("updated notes"));
            Assert.That(meal.UpdatedOnUtc, Is.EqualTo(updatedAt));

            MealPlanUpdatedDomainEvent ev = meal.GetDomainEvents().OfType<MealPlanUpdatedDomainEvent>().Single();
            Assert.That(ev.MealPlanId, Is.EqualTo(meal.Id));
        });
    }

    /// <summary>
    /// Ensures Update() forces servings <= 0 to become 1.
    /// </summary>
    [Test]
    public void Update_InvalidServings_FallbacksToOne()
    {
        var meal = MealPlan.Create(
            Guid.NewGuid(),
            "Test Meal",
            MealType.Snack,
            new DateOnly(2025, 3, 3),
            5,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow
        );


        meal.Update(
            MealType.Snack,
            new DateOnly(2025, 4, 4),
            servings: 0,
            notes: "notes",
            DateTime.UtcNow
        );

        Assert.That(meal.Servings, Is.EqualTo(1));
    }
}
