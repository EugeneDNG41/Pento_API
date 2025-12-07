using Pento.Domain.Abstractions;

namespace Pento.Domain.Recipes.Events;

public sealed class RecipeUpdatedDomainEvent(Guid recipeId)
    : DomainEvent
{
    public Guid RecipeId { get; } = recipeId;
}
