using NUnit.Framework;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;
using Pento.Domain.RecipeDirections.Events;

namespace Pento.Domain.UnitTests;

internal sealed class RecipeDirectionTests
{
    /// <summary>
    /// Verifies that Create() initializes all properties correctly
    /// and sets CreatedOnUtc and UpdatedOnUtc properly.
    /// No domain event is raised by Create().
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesDirectionCorrectly()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        int step = 1;
        string desc = "Chop the onions";
        var image = new Uri("https://example.com/step1.jpg");
        DateTime now = DateTime.UtcNow;

        // Act
        var direction = RecipeDirection.Create(recipeId, step, desc, image, now);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(direction.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(direction.RecipeId, Is.EqualTo(recipeId));
            Assert.That(direction.StepNumber, Is.EqualTo(step));
            Assert.That(direction.Description, Is.EqualTo(desc));
            Assert.That(direction.ImageUrl, Is.EqualTo(image));
            Assert.That(direction.CreatedOnUtc, Is.EqualTo(now));
            Assert.That(direction.UpdatedOnUtc, Is.EqualTo(now));

            // Create() does NOT raise events for this entity
            Assert.That(direction.GetDomainEvents().Count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Verifies Update() modifies Description and UpdatedOnUtc,
    /// and raises a RecipeDirectionUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void Update_ValidInput_UpdatesPropertiesAndRaisesEvent()
    {
        // Arrange
        var direction = RecipeDirection.Create(
            Guid.NewGuid(),
            1,
            "Old description",
            null,
            DateTime.UtcNow);


        string newDesc = "New description";
        DateTime newTime = DateTime.UtcNow.AddMinutes(5);

        // Act
        direction.Update(newDesc, newTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(direction.Description, Is.EqualTo(newDesc));
            Assert.That(direction.UpdatedOnUtc, Is.EqualTo(newTime));

            IReadOnlyList<IDomainEvent> events = direction.GetDomainEvents();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<RecipeDirectionUpdatedDomainEvent>());

            var ev = (RecipeDirectionUpdatedDomainEvent)events[0];
            Assert.That(ev.RecipeDirectionId, Is.EqualTo(direction.Id));
        });
    }

    /// <summary>
    /// Verifies UpdateImageUrl updates the image URL and UpdatedOnUtc,
    /// and does NOT raise any domain event.
    /// </summary>
    [Test]
    public void UpdateImageUrl_ValidInput_UpdatesImage_NoEventRaised()
    {
        // Arrange
        var direction = RecipeDirection.Create(
            Guid.NewGuid(),
            2,
            "Saute veggies",
            null,
            DateTime.UtcNow);


        var newUrl = new Uri("https://example.com/newimg.jpg");
        DateTime newTime = DateTime.UtcNow.AddMinutes(10);

        // Act
        direction.UpdateImageUrl(newUrl, newTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(direction.ImageUrl, Is.EqualTo(newUrl));
            Assert.That(direction.UpdatedOnUtc, Is.EqualTo(newTime));

            // No event from UpdateImageUrl()
            Assert.That(direction.GetDomainEvents().Count, Is.EqualTo(0));
        });
    }
}
