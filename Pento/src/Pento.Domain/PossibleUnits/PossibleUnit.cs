using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.PossibleUnits.Events;

namespace Pento.Domain.PossibleUnits;

public sealed class PossibleUnit : Entity
{
    public PossibleUnit(
        Guid id,
        Guid unitId,
        Guid foodRefId,
        bool isDefault,
        DateTime createdOnUtc)
        : base(id)
    {
        UnitId = unitId;
        FoodRefId = foodRefId;
        IsDefault = isDefault;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private PossibleUnit() { }

    public Guid UnitId { get; private set; }

    public Guid FoodRefId { get; private set; }

    public bool IsDefault { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }

    public static PossibleUnit Create(Guid unitId, Guid foodRefId, bool isDefault, DateTime utcNow)
    {
        var possibleUnit = new PossibleUnit(
            Guid.CreateVersion7(),
            unitId,
            foodRefId,
            isDefault,
            utcNow
        );
        possibleUnit.Raise(new PossibleUnitCreatedDomainEvent(possibleUnit.Id, foodRefId, unitId));

        return possibleUnit;
    }

    public void SetAsDefault(bool isDefault, DateTime utcNow)
    {
        if (IsDefault == isDefault)
        {
            return; 
        }

        IsDefault = isDefault;
        UpdatedOnUtc = utcNow;

        Raise(new PossibleUnitUpdatedDomainEvent(Id));
    }
}
