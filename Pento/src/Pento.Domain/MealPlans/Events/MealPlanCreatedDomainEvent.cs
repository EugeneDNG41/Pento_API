using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlans.Events;
public sealed class MealPlanCreatedDomainEvent(Guid MealPlanId) : DomainEvent
{
    public Guid MealPlanId { get; init; } = MealPlanId;
}
