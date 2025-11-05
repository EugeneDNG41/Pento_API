using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodDietaryTags;
public sealed class FoodDietaryTag : Entity
{
    private FoodDietaryTag() { }

    public FoodDietaryTag(Guid id, Guid foodReferenceId, Guid dietaryTagId)
        : base(id)
    {
        FoodReferenceId = foodReferenceId;
        DietaryTagId = dietaryTagId;
    }

    public Guid FoodReferenceId { get; private set; }
    public Guid DietaryTagId { get; private set; }

    public static FoodDietaryTag Create(Guid foodReferenceId, Guid dietaryTagId)
    {
        return new FoodDietaryTag(Guid.CreateVersion7(), foodReferenceId, dietaryTagId);
    }
}
