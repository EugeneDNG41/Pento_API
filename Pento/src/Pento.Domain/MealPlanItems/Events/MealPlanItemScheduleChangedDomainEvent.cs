using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlanItems.Events;
public sealed class MealPlanItemScheduleChangedDomainEvent(Guid MealPlanItemId, List<DateTime> NewSchedule) : DomainEvent
{
    public Guid MealPlanItemId { get; init; } = MealPlanItemId;
    public List<DateTime> NewSchedule { get; init; } = NewSchedule;
}
