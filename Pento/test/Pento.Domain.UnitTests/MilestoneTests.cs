using NUnit.Framework;
using Pento.Domain.Milestones;

namespace Pento.Domain.UnitTests;

internal sealed class MilestoneTests
{
    /// <summary>
    /// Ensures Create() initializes properties correctly.
    /// </summary>
    [Test]
    public void Create_ValidInputs_InitializesProperties()
    {
        // Arrange
        string name = "Zero Waste Hero";
        string description = "Reduce food waste";
        var iconUrl = new Uri("https://example.com/icon.png");

        // Act
        var milestone = Milestone.Create(name, description, iconUrl);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(milestone.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(milestone.Name, Is.EqualTo(name));
            Assert.That(milestone.Description, Is.EqualTo(description));
            Assert.That(milestone.IconUrl, Is.EqualTo(iconUrl));
            Assert.That(milestone.IsActive, Is.False); // default
        });
    }

    /// <summary>
    /// Ensures UpdateDetails updates only non-empty values.
    /// </summary>
    [Test]
    public void UpdateDetails_ValidInputs_UpdatesFields()
    {
        // Arrange
        var milestone = Milestone.Create(
            "Old Name",
            "Old Description",
            null);

        // Act
        milestone.UpdateDetails("New Name", "New Description");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(milestone.Name, Is.EqualTo("New Name"));
            Assert.That(milestone.Description, Is.EqualTo("New Description"));
        });
    }

    /// <summary>
    /// Ensures UpdateDetails ignores null or whitespace inputs.
    /// </summary>
    [Test]
    public void UpdateDetails_NullOrWhitespace_IgnoresChanges()
    {
        // Arrange
        var milestone = Milestone.Create(
            "Name",
            "Description",
            null);

        // Act
        milestone.UpdateDetails("   ", null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(milestone.Name, Is.EqualTo("Name"));
            Assert.That(milestone.Description, Is.EqualTo("Description"));
        });
    }

    /// <summary>
    /// Ensures UpdateIconUrl updates when different.
    /// </summary>
    [Test]
    public void UpdateIconUrl_DifferentValue_UpdatesIcon()
    {
        // Arrange
        var milestone = Milestone.Create(
            "Name",
            "Description",
            null);

        var newIcon = new Uri("https://example.com/new.png");

        // Act
        milestone.UpdateIconUrl(newIcon);

        // Assert
        Assert.That(milestone.IconUrl, Is.EqualTo(newIcon));
    }

    /// <summary>
    /// Ensures UpdateIconUrl ignores same value.
    /// </summary>
    [Test]
    public void UpdateIconUrl_SameValue_NoChange()
    {
        // Arrange
        var icon = new Uri("https://example.com/icon.png");
        var milestone = Milestone.Create(
            "Name",
            "Description",
            icon);

        // Act
        milestone.UpdateIconUrl(icon);

        // Assert
        Assert.That(milestone.IconUrl, Is.EqualTo(icon));
    }

    /// <summary>
    /// Ensures Enable() sets IsActive = true and raises event.
    /// </summary>
    [Test]
    public void Enable_SetsActiveAndRaisesEvent()
    {
        // Arrange
        var milestone = Milestone.Create(
            "Milestone",
            "Desc",
            null,
            isActive: false);


        // Act
        milestone.Enable();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(milestone.IsActive, Is.True);

            MilestoneEnabledOrUpdatedDomainEvent ev = milestone.GetDomainEvents()
                .OfType<MilestoneEnabledOrUpdatedDomainEvent>()
                .Single();

            Assert.That(ev.MilestoneId, Is.EqualTo(milestone.Id));
        });
    }

    /// <summary>
    /// Ensures Disable() sets IsActive = false.
    /// No domain event expected.
    /// </summary>
    [Test]
    public void Disable_SetsInactive()
    {
        // Arrange
        var milestone = Milestone.Create(
            "Milestone",
            "Desc",
            null,
            isActive: true);

        // Act
        milestone.Disable();

        // Assert
        Assert.That(milestone.IsActive, Is.False);
    }
}
