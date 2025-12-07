using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlans.Events;

public sealed class MealPlanUpdatedDomainEvent(Guid MealPlanId) : DomainEvent
{
    public Guid MealPlanId { get; init; } = MealPlanId;
}
