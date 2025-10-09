﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Recipes.Get;
public sealed record RecipeResponse(
    Guid Id,
    string Title,
    string? Description,
    int? PrepTimeMinutes,
    int? CookTimeMinutes,
    int TotalMinutes,
    string? Notes,
    int? Servings,
    string? DifficultyLevel,
    int? CaloriesPerServing,
    Uri? ImageUrl,
    Guid CreatedBy,
    bool IsPublic,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
