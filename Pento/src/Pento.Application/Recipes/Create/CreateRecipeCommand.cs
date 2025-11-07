using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Create;
public sealed record CreateRecipeCommand(
    string Title,
    string Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    string? Notes,
    int? Servings,
    DifficultyLevel? DifficultyLevel,
    Uri? ImageUrl,
    Guid? CreatedBy,
    bool IsPublic
) : ICommand<Guid>;
