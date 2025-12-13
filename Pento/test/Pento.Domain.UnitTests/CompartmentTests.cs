using NUnit.Framework;
using Pento.Domain.Compartments;

namespace Pento.Domain.UnitTests;

internal sealed class CompartmentTests
{
    /// <summary>
    /// Ensures Create() initializes properties correctly
    /// and raises CompartmentCreatedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesCompartmentAndRaisesEvent()
    {
        // Arrange
        string name = "Top Shelf";
        var storageId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        string? notes = "For dry food";
        var userId = Guid.NewGuid();

        // Act
        var compartment = Compartment.Create(
            name,
            storageId,
            householdId,
            notes,
            userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(compartment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(compartment.Name, Is.EqualTo(name));
            Assert.That(compartment.StorageId, Is.EqualTo(storageId));
            Assert.That(compartment.HouseholdId, Is.EqualTo(householdId));
            Assert.That(compartment.Notes, Is.EqualTo(notes));

            CompartmentCreatedDomainEvent ev = compartment.GetDomainEvents()
                .OfType<CompartmentCreatedDomainEvent>()
                .Single();

            Assert.That(ev.CompartmentId, Is.EqualTo(compartment.Id));
            Assert.That(ev.UserId, Is.EqualTo(userId));
        });
    }

    /// <summary>
    /// Ensures AutoCreate() initializes properties correctly
    /// and does NOT raise any domain event.
    /// </summary>
    [Test]
    public void AutoCreate_ValidInputs_CreatesCompartmentWithoutEvent()
    {
        // Arrange
        string name = "Default";
        var storageId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        string? notes = null;

        // Act
        var compartment = Compartment.AutoCreate(
            name,
            storageId,
            householdId,
            notes);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(compartment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(compartment.Name, Is.EqualTo(name));
            Assert.That(compartment.StorageId, Is.EqualTo(storageId));
            Assert.That(compartment.HouseholdId, Is.EqualTo(householdId));
            Assert.That(compartment.Notes, Is.Null);
        });
    }

    /// <summary>
    /// Ensures UpdateName() updates the Name property.
    /// </summary>
    [Test]
    public void UpdateName_ValidInput_UpdatesName()
    {
        // Arrange
        var compartment = Compartment.AutoCreate(
            "Old Name",
            Guid.NewGuid(),
            Guid.NewGuid(),
            null);

        // Act
        compartment.UpdateName("New Name");

        // Assert
        Assert.That(compartment.Name, Is.EqualTo("New Name"));
    }

    /// <summary>
    /// Ensures UpdateNotes() updates Notes property.
    /// </summary>
    [Test]
    public void UpdateNotes_ValidInput_UpdatesNotes()
    {
        // Arrange
        var compartment = Compartment.AutoCreate(
            "Compartment",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Old notes");

        // Act
        compartment.UpdateNotes("Updated notes");

        // Assert
        Assert.That(compartment.Notes, Is.EqualTo("Updated notes"));
    }

    /// <summary>
    /// Ensures UpdateNotes() supports clearing notes.
    /// </summary>
    [Test]
    public void UpdateNotes_Null_ClearsNotes()
    {
        // Arrange
        var compartment = Compartment.AutoCreate(
            "Compartment",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Some notes");

        // Act
        compartment.UpdateNotes(null);

        // Assert
        Assert.That(compartment.Notes, Is.Null);
    }
}
