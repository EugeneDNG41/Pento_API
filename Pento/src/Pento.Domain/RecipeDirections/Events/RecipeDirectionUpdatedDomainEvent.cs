using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeDirections.Events;

public sealed class RecipeDirectionUpdatedDomainEvent(Guid recipeDirectionId) : DomainEvent
{
    public Guid RecipeDirectionId { get; init; } = recipeDirectionId;
}
