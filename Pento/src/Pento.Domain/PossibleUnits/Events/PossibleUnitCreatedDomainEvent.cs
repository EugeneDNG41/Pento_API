using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.PossibleUnits.Events;
public sealed class PossibleUnitCreatedDomainEvent(Guid possibleUnitId, Guid foodRefId, Guid unitId)
    : DomainEvent
{
    public Guid PossibleUnitId { get; init; } = possibleUnitId;
    public Guid FoodRefId { get; init; } = foodRefId;
    public Guid UnitId { get; init; } = unitId;
}
