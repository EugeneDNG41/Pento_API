using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Recipes;
public sealed class Recipe: Entity
{
    public Recipe(
        Guid id,
        string title,
        string? description,
        TimeRequirement recipetime,
        string? notes,
        int? servings,
        DifficultyLevel? difficultyLevel,
        Uri? imageUrl,
        Guid createdBy,
        bool isPublic,
        DateTime createdOnUtc)
        : base(id)
    {
        Title = title;
        Description = description;
        Notes = notes;
        RecipeTime = recipetime;
        Servings = servings;
        DifficultyLevel = difficultyLevel;
        ImageUrl = imageUrl;
        CreatedBy = createdBy;
        IsPublic = isPublic;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }
    private Recipe() { }
    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public int? PrepTimeMinutes { get; private set; }

    public int? CookTimeMinutes { get; private set; }

    public string? Notes { get; private set; }

    public int? Servings { get; private set; }

    public DifficultyLevel? DifficultyLevel { get; private set; }

    public int? CaloriesPerServing { get; private set; }

    public Uri? ImageUrl { get; private set; }

    public Guid CreatedBy { get; private set; }

    public TimeRequirement RecipeTime { get; private set; } 
    public bool IsPublic { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }
}
