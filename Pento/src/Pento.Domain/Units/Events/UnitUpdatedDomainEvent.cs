using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Units.Events;
public sealed class UnitUpdatedDomainEvent(Guid unitId, string name) : DomainEvent
{
    public Guid UnitId { get; init; } = unitId;
    public string Name { get; init; } = name;
}
