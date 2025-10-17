using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Units.Events;

namespace Pento.Domain.Units;
public sealed class Unit : Entity
{
    public Unit(
        Guid id,
        string name,
        string abbreviation,
        decimal toBaseFactor,
        DateTime createdOnUtc)
        : base(id)
    {
        Name = name;
        Abbreviation = abbreviation;
        ToBaseFactor = toBaseFactor;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private Unit() { }

    public string Name { get; private set; }

    public string Abbreviation { get; private set; }

    public decimal ToBaseFactor { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }

    public static Unit Create(
        string name,
        string abbreviation,
        decimal toBaseFactor,
        DateTime utcNow)
    {
        var unit = new Unit(
            Guid.CreateVersion7(),
            name,
            abbreviation,
            toBaseFactor,
            utcNow);

        unit.Raise(new UnitCreatedDomainEvent(unit.Id, name));

        return unit;
    }

    public void Update(string name, string abbreviation, decimal toBaseFactor, DateTime utcNow)
    {
        Name = name;
        Abbreviation = abbreviation;
        ToBaseFactor = toBaseFactor;
        UpdatedOnUtc = utcNow;

        Raise(new UnitUpdatedDomainEvent(Id, name));
    }
}
