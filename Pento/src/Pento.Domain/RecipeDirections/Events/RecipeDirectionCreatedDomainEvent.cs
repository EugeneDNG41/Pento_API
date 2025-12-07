using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeDirections.Events;

public sealed class RecipeDirectionCreatedDomainEvent(Guid recipeDirectionId, Guid recipeId) : DomainEvent
{
    public Guid RecipeDirectionId { get; init; } = recipeDirectionId;
    public Guid RecipeId { get; init; } = recipeId;
}

