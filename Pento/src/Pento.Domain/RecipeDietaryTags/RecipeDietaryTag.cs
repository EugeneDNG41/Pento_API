using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeDietaryTags;
public sealed class RecipeDietaryTag : Entity
{
    private RecipeDietaryTag() { }

    public RecipeDietaryTag(Guid id, Guid recipeId, Guid dietaryTagId)
        : base(id)
    {
        RecipeId = recipeId;
        DietaryTagId = dietaryTagId;
    }

    public Guid RecipeId { get; private set; }
    public Guid DietaryTagId { get; private set; }

    public static RecipeDietaryTag Create(Guid recipeId, Guid dietaryTagId)
    {
        return new RecipeDietaryTag(Guid.CreateVersion7(), recipeId, dietaryTagId);
    }
}
