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
        UnitType type)
        : base(id)
    {
        Name = name;
        Abbreviation = abbreviation;
        ToBaseFactor = toBaseFactor;
        Type = type;
    }

    private Unit() { }

    public string Name { get; private set; }

    public string Abbreviation { get; private set; }

    public decimal ToBaseFactor { get; private set; }
    public UnitType Type { get; private set; }

    public static Unit Create(
        string name,
        string abbreviation,
        decimal toBaseFactor,
        UnitType type)
    {
        var unit = new Unit(
            Guid.CreateVersion7(),
            name,
            abbreviation,
            toBaseFactor,
            type);

        unit.Raise(new UnitCreatedDomainEvent(unit.Id, name));

        return unit;
    }

    public void Update(string name, string abbreviation, decimal toBaseFactor, UnitType type)
    {
        Name = name;
        Abbreviation = abbreviation;
        ToBaseFactor = toBaseFactor;
        Type = type;

        Raise(new UnitUpdatedDomainEvent(Id, name));
    }
}
public enum UnitType
{
    Weight,
    Volume,
    Count
}
