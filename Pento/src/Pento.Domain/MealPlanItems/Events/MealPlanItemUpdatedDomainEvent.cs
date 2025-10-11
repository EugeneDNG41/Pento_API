using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlanItems.Events;
public sealed class MealPlanItemUpdatedDomainEvent(Guid MealPlanItemId ) : DomainEvent
{
    public Guid MealPlanItemId { get; init; } = MealPlanItemId;
}
