using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.PossibleUnits.Events;
public sealed class PossibleUnitUpdatedDomainEvent(Guid possibleUnitId)
    : DomainEvent
{
    public Guid PossibleUnitId { get; init; } = possibleUnitId;
}
