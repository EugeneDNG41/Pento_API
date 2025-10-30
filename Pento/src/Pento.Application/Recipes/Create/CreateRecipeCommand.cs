using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Create;
public sealed record CreateRecipeCommand(
    string Title,
    string? Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    string? Notes,
    int? Servings,
    string? DifficultyLevel,
    int? CaloriesPerServing,
    Uri? ImageUrl,
    Guid? CreatedBy,
    bool IsPublic
) : ICommand<Guid>;
