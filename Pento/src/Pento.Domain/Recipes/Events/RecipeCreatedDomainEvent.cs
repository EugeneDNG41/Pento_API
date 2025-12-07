using Pento.Domain.Abstractions;

namespace Pento.Domain.Recipes.Events;

public sealed class RecipeCreatedDomainEvent(Guid recipeId, Guid userId) : DomainEvent
{
    public Guid RecipeId { get; } = recipeId;
    public Guid UserId { get; } = userId;
}
