using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Get.Public;
public sealed record GetPublicRecipeQuery(
    Guid RecipeId,
    string? Include
) : IQuery<RecipeDetailResponse>;

