using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeIngredients.Events;

public sealed class RecipeIngredientUpdatedDomainEvent(Guid RecipeIngredientId)
    : DomainEvent
{
    public Guid RecipeIngredientId { get; init; } = RecipeIngredientId;
}
