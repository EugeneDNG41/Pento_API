using NUnit.Framework;
using Pento.Domain.Units;
using Pento.Domain.Units.Events;

namespace Pento.Domain.UnitTests;

internal sealed class UnitTests
{
    /// <summary>
    /// Verifies that Create() initializes all properties and raises UnitCreatedDomainEvent.
    /// </summary>
    [Test]
    public void Create_ValidInputs_CreatesUnitAndRaisesEvent()
    {
        // Arrange
        string name = "Gram";
        string abbr = "g";
        decimal factor = 1m;
        UnitType type = UnitType.Weight;

        // Act
        var unit = Unit.Create(name, abbr, factor, type);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(unit.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(unit.Name, Is.EqualTo(name));
            Assert.That(unit.Abbreviation, Is.EqualTo(abbr));
            Assert.That(unit.ToBaseFactor, Is.EqualTo(factor));
            Assert.That(unit.Type, Is.EqualTo(type));

            IReadOnlyList<Abstractions.IDomainEvent> events = unit.GetDomainEvents();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<UnitCreatedDomainEvent>());

            var ev = (UnitCreatedDomainEvent)events[0];
            Assert.That(ev.UnitId, Is.EqualTo(unit.Id));
            Assert.That(ev.Name, Is.EqualTo(name));
        });
    }

    /// <summary>
    /// Verifies Update() overwrites all fields and raises UnitUpdatedDomainEvent.
    /// </summary>

}
