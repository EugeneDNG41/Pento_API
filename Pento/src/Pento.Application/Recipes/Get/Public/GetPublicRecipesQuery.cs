using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get.Public;
public sealed record GetPublicRecipesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    DifficultyLevel? DifficultyLevel = null,
    string? Search = null,
    string? Sort = "newest"
) : IQuery<PagedList<RecipeResponse>>;
