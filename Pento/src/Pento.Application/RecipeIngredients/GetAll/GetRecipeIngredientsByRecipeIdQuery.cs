using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.Get;

namespace Pento.Application.RecipeIngredients.GetAll;
public sealed record GetRecipeIngredientsByRecipeIdQuery(Guid RecipeId)
    : IQuery<RecipeWithIngredientsResponse>;
