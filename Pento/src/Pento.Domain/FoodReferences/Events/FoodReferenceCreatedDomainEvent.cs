using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodReferences.Events;
public sealed class FoodReferenceCreatedDomainEvent(Guid foodReferenceId) : DomainEvent
{
    public Guid FoodReferenceId { get; init; } = foodReferenceId;

}
