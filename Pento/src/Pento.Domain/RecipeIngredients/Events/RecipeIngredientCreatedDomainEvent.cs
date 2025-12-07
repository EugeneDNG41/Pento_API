using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeIngredients.Events;

public sealed class RecipeIngredientCreatedDomainEvent(Guid RecipeIngredientId, Guid RecipeId)
    : DomainEvent
{
    public Guid RecipeIngredientId { get; init; } = RecipeIngredientId;
    public Guid RecipeId { get; init; } = RecipeId;
}
