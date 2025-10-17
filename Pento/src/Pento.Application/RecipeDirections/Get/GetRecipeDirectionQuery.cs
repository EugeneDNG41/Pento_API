using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Get;

public sealed record GetRecipeDirectionQuery(Guid RecipeDirectionId)
    : IQuery<RecipeDirectionResponse>;
