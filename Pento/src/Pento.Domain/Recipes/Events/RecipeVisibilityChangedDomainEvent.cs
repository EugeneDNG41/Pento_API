using Pento.Domain.Abstractions;

namespace Pento.Domain.Recipes.Events;

public sealed class RecipeVisibilityChangedDomainEvent(Guid recipeId, bool isPublic)
    : DomainEvent
{
    public Guid RecipeId { get; } = recipeId;
    public bool IsPublic { get; } = isPublic;
}
