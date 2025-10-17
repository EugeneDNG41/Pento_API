using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeDirections.Events;

public sealed class RecipeDirectionCreatedDomainEvent(Guid recipeDirectionId, Guid recipeId) : DomainEvent
{
    public Guid RecipeDirectionId { get; init; } = recipeDirectionId;
    public Guid RecipeId { get; init; } = recipeId;
}

public sealed class RecipeDirectionUpdatedDomainEvent(Guid recipeDirectionId) : DomainEvent
{
    public Guid RecipeDirectionId { get; init; } = recipeDirectionId;
}
