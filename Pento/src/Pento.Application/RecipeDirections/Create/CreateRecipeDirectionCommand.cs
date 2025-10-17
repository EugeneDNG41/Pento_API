using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeDirections.Create;
public sealed record CreateRecipeDirectionCommand(
    Guid RecipeId,
    int StepNumber,
    string Description,
    string? ImageUrl
) : ICommand<Guid>;
