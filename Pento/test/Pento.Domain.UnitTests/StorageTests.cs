using NUnit.Framework;
using Pento.Domain.Storages;

namespace Pento.Domain.UnitTests;

internal sealed class StorageTests
{
    /// <summary>
    /// Ensures Create() initializes properties correctly
    /// and raises StorageCreatedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesStorageAndRaisesEvent()
    {
        // Arrange
        string name = "Main Fridge";
        var householdId = Guid.NewGuid();
        StorageType type = StorageType.Fridge;
        string? notes = "Top floor";
        var userId = Guid.NewGuid();

        // Act
        var storage = Storage.Create(
            name,
            householdId,
            type,
            notes,
            userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(storage.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(storage.Name, Is.EqualTo(name));
            Assert.That(storage.HouseholdId, Is.EqualTo(householdId));
            Assert.That(storage.Type, Is.EqualTo(type));
            Assert.That(storage.Notes, Is.EqualTo(notes));

            StorageCreatedDomainEvent ev = storage.GetDomainEvents()
                .OfType<StorageCreatedDomainEvent>()
                .Single();

            Assert.That(ev.StorageId, Is.EqualTo(storage.Id));
            Assert.That(ev.UserId, Is.EqualTo(userId));
        });
    }

    /// <summary>
    /// Ensures AutoCreate() initializes properties correctly
    /// and does NOT raise any domain event.
    /// </summary>
    [Test]
    public void AutoCreate_ValidInputs_CreatesStorageWithoutEvent()
    {
        // Arrange
        string name = "Default Pantry";
        var householdId = Guid.NewGuid();
        StorageType type = StorageType.Pantry;
        string? notes = null;

        // Act
        var storage = Storage.AutoCreate(
            name,
            householdId,
            type,
            notes);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(storage.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(storage.Name, Is.EqualTo(name));
            Assert.That(storage.HouseholdId, Is.EqualTo(householdId));
            Assert.That(storage.Type, Is.EqualTo(type));
            Assert.That(storage.Notes, Is.Null);
        });
    }

    /// <summary>
    /// Ensures UpdateName() updates Name.
    /// </summary>
    [Test]
    public void UpdateName_ValidInput_UpdatesName()
    {
        // Arrange
        var storage = Storage.AutoCreate(
            "Old Name",
            Guid.NewGuid(),
            StorageType.Freezer,
            null);

        // Act
        storage.UpdateName("New Name");

        // Assert
        Assert.That(storage.Name, Is.EqualTo("New Name"));
    }

    /// <summary>
    /// Ensures UpdateNotes() updates Notes.
    /// </summary>
    [Test]
    public void UpdateNotes_ValidInput_UpdatesNotes()
    {
        // Arrange
        var storage = Storage.AutoCreate(
            "Storage",
            Guid.NewGuid(),
            StorageType.Pantry,
            "Old notes");

        // Act
        storage.UpdateNotes("Updated notes");

        // Assert
        Assert.That(storage.Notes, Is.EqualTo("Updated notes"));
    }

    /// <summary>
    /// Ensures UpdateNotes() supports clearing notes.
    /// </summary>
    [Test]
    public void UpdateNotes_Null_ClearsNotes()
    {
        // Arrange
        var storage = Storage.AutoCreate(
            "Storage",
            Guid.NewGuid(),
            StorageType.Pantry,
            "Some notes");

        // Act
        storage.UpdateNotes(null);

        // Assert
        Assert.That(storage.Notes, Is.Null);
    }
}
