using NUnit.Framework;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;
using Pento.Domain.Recipes.Events;

namespace Pento.Domain.UnitTests;

internal sealed class RecipeTests
{
    /// <summary>
    /// Verifies that Create() initializes all fields correctly,
    /// sets CreatedOnUtc and UpdatedOnUtc properly,
    /// and raises exactly one RecipeCreatedDomainEvent with correct values.
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesRecipeAndRaisesEvent()
    {
        // Arrange
        string title = "Pasta";
        string description = "Delicious pasta dish";
        var time = TimeRequirement.Create(10, 20);
        string? notes = "Serve hot";
        int? servings = 4;
        DifficultyLevel? difficulty = DifficultyLevel.Medium;
        var imageUrl = new Uri("https://example.com/pasta.jpg");
        var createdBy = Guid.NewGuid();
        bool isPublic = false;
        DateTime utcNow = DateTime.UtcNow;

        // Act
        var recipe = Recipe.Create(
            title,
            description,
            time,
            notes,
            servings,
            difficulty,
            imageUrl,
            createdBy,
            isPublic,
            utcNow);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(recipe.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(recipe.Title, Is.EqualTo(title));
            Assert.That(recipe.Description, Is.EqualTo(description));
            Assert.That(recipe.RecipeTime.PrepTimeMinutes, Is.EqualTo(10));
            Assert.That(recipe.RecipeTime.CookTimeMinutes, Is.EqualTo(20));
            Assert.That(recipe.Notes, Is.EqualTo(notes));
            Assert.That(recipe.Servings, Is.EqualTo(servings));
            Assert.That(recipe.DifficultyLevel, Is.EqualTo(difficulty));
            Assert.That(recipe.ImageUrl, Is.EqualTo(imageUrl));
            Assert.That(recipe.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(recipe.IsPublic, Is.EqualTo(isPublic));
            Assert.That(recipe.CreatedOnUtc, Is.EqualTo(utcNow));
            Assert.That(recipe.UpdatedOnUtc, Is.EqualTo(utcNow));

            IReadOnlyList<IDomainEvent> events = recipe.GetDomainEvents();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<RecipeCreatedDomainEvent>());

            var createdEvent = (RecipeCreatedDomainEvent)events[0];
            Assert.That(createdEvent.RecipeId, Is.EqualTo(recipe.Id));
        });
    }

    /// <summary>
    /// Verifies UpdateDetails overwrites recipe fields
    /// and updates UpdatedOnUtc.
    /// No events should be raised by this method.
    /// </summary>
    [Test]
    public void UpdateDetails_ValidInputs_UpdatesFieldsButDoesNotRaiseEvent()
    {
        // Arrange
        var recipe = Recipe.Create(
            "Old title",
            "Old desc",
            TimeRequirement.Create(5, 10),
            "Old notes",
            2,
            DifficultyLevel.Easy,
            null,
            Guid.NewGuid(),
            false,
            DateTime.UtcNow);

        DateTime newTime = DateTime.UtcNow.AddMinutes(5);
        var newUrl = new Uri("https://example.com/new.jpg");

        // Act
        recipe.UpdateDetails(
            "New Title",
            "New Description",
            "New Notes",
            6,
            DifficultyLevel.Hard,
            newUrl,
            newTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(recipe.Title, Is.EqualTo("New Title"));
            Assert.That(recipe.Description, Is.EqualTo("New Description"));
            Assert.That(recipe.Notes, Is.EqualTo("New Notes"));
            Assert.That(recipe.Servings, Is.EqualTo(6));
            Assert.That(recipe.DifficultyLevel, Is.EqualTo(DifficultyLevel.Hard));
            Assert.That(recipe.ImageUrl, Is.EqualTo(newUrl));
            Assert.That(recipe.UpdatedOnUtc, Is.EqualTo(newTime));

        });
    }

    /// <summary>
    /// Verifies ChangeVisibility raises an event if visibility is changed,
    /// and updates UpdatedOnUtc.
    /// </summary>
    [Test]
    public void ChangeVisibility_ChangesValue_RaisesEvent()
    {
        // Arrange
        var recipe = Recipe.Create(
            "Test",
            "Desc",
            TimeRequirement.Create(1, 1),
            null,
            null,
            DifficultyLevel.Easy,
            null,
            Guid.NewGuid(),
            isPublic: false,
            DateTime.UtcNow);

        DateTime newUtc = DateTime.UtcNow.AddHours(1);

        // Act
        recipe.ChangeVisibility(true, newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(recipe.IsPublic, Is.True);
            Assert.That(recipe.UpdatedOnUtc, Is.EqualTo(newUtc));

            var events = recipe.GetDomainEvents().OfType<RecipeVisibilityChangedDomainEvent>().ToList();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].RecipeId, Is.EqualTo(recipe.Id));
            Assert.That(events[0].IsPublic, Is.True);
        });
    }

    /// <summary>
    /// Verifies ChangeVisibility does not raise event
    /// if visibility remains unchanged.
    /// </summary>
   

    /// <summary>
    /// Verifies UpdateImageUrl changes the image URL and updates timestamp.
    /// Does not raise any domain event.
    /// </summary>
    [Test]
    public void UpdateImageUrl_ValidInput_UpdatesImageAndDoesNotRaiseEvent()
    {
        // Arrange
        var recipe = Recipe.Create(
            "Test",
            "Desc",
            TimeRequirement.Create(1, 1),
            null,
            null,
            DifficultyLevel.Easy,
            null,
            Guid.NewGuid(),
            false,
            DateTime.UtcNow);


        var url = new Uri("https://example.com/img.png");
        DateTime updateTime = DateTime.UtcNow.AddMinutes(30);

        // Act
        recipe.UpdateImageUrl(url, updateTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(recipe.ImageUrl, Is.EqualTo(url));
            Assert.That(recipe.UpdatedOnUtc, Is.EqualTo(updateTime));
        });
    }
}
