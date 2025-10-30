using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get;
public sealed record GetAllRecipesQuery()
    : IQuery<IReadOnlyList<RecipeResponse>>;
