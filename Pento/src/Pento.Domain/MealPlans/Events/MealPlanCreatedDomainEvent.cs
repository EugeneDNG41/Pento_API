using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlans.Events;

public sealed class MealPlanCreatedDomainEvent(Guid MealPlanId, Guid userId) : DomainEvent
{
    public Guid MealPlanId { get; init; } = MealPlanId;
    public Guid UserId { get; init; } = userId;
}
