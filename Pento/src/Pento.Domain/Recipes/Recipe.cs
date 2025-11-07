using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeMedia;
using Pento.Domain.Recipes.Events;
using Pento.Domain.Roles;

namespace Pento.Domain.Recipes;
public sealed class Recipe: Entity
{
    public Recipe(
        Guid id,
        string title,
        string description,
        TimeRequirement recipetime,
        string? notes,
        int? servings,
        DifficultyLevel? difficultyLevel,
        Uri? imageUrl,
        Guid? createdBy,
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

    public string Description { get; private set; }

    public int PrepTimeMinutes { get; private set; }

    public int CookTimeMinutes { get; private set; }

    public string? Notes { get; private set; }

    public int? Servings { get; private set; }

    public DifficultyLevel? DifficultyLevel { get; private set; }

    public int? CaloriesPerServing { get; private set; }

    public Uri? ImageUrl { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public TimeRequirement RecipeTime { get; private set; } 
    public bool IsPublic { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }
    public ICollection<RecipeMedia.RecipeMedia> Media { get; private set; }

    public static Recipe Create(
    string title,
    string description,
    TimeRequirement recipeTime,
    string? notes,
    int? servings,
    DifficultyLevel? difficultyLevel,
    Uri? imageUrl,
    Guid? createdBy,
    bool isPublic,
    DateTime utcNow)
    {
        var recipe = new Recipe(
            Guid.NewGuid(),
            title,
            description,
            recipeTime,
            notes,
            servings,
            difficultyLevel,
            imageUrl,
            createdBy,
            isPublic,
            utcNow);

        recipe.Raise(new RecipeCreatedDomainEvent(recipe.Id));

        return recipe;
    }
    public void UpdateDetails(
    string title,
    string description,
    string? notes,
    int? servings,
    DifficultyLevel? difficultyLevel,
    Uri? imageUrl,
    DateTime utcNow)
    {
        Title = title;
        Description = description;
        Notes = notes;
        Servings = servings;
        DifficultyLevel = difficultyLevel;
        ImageUrl = imageUrl;
        UpdatedOnUtc = utcNow;

        Raise(new RecipeUpdatedDomainEvent(Id));
    }

    public void ChangeVisibility(bool isPublic, DateTime utcNow)
    {
        if (IsPublic == isPublic)
        {
            return;
        }

        IsPublic = isPublic;
        UpdatedOnUtc = utcNow;

        Raise(new RecipeVisibilityChangedDomainEvent(Id, isPublic));
    }
}
