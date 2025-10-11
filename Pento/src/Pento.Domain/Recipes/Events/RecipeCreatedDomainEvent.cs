using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Recipes.Events;
public sealed class RecipeCreatedDomainEvent(Guid recipeId)
    : DomainEvent
{
    public Guid RecipeId { get; } = recipeId;
}
