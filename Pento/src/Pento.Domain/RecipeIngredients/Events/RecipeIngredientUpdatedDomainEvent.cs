using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeIngredients.Events;
public sealed class RecipeIngredientUpdatedDomainEvent(Guid RecipeIngredientId)
    : DomainEvent
{
    public Guid RecipeIngredientId { get; init; } = RecipeIngredientId;
}
