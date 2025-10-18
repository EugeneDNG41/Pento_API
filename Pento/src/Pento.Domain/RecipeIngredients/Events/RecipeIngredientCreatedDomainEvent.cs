using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeIngredients.Events;
public sealed class RecipeIngredientCreatedDomainEvent(Guid RecipeIngredientId, Guid RecipeId)
    : DomainEvent
{
    public Guid RecipeIngredientId { get; init; } = RecipeIngredientId;
    public Guid RecipeId { get; init; } = RecipeId;
}
