using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections.Events;

namespace Pento.Domain.RecipeDirections;
public sealed class RecipeDirection : Entity
{
    public RecipeDirection(
        Guid id,
        Guid recipeId,
        int stepNumber,
        string description,
        Uri? imageUrl,
        DateTime createdOnUtc)
        : base(id)
    {
 
        RecipeId = recipeId;
        StepNumber = stepNumber;
        Description = description;
        ImageUrl = imageUrl;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private RecipeDirection() { }

    public Guid RecipeId { get; private set; }

    public int StepNumber { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Uri? ImageUrl { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }
    public static RecipeDirection Create(Guid recipeId, int stepNumber, string description, Uri? imageUrl, DateTime utcNow)
    {
        var direction = new RecipeDirection(Guid.CreateVersion7(), recipeId, stepNumber, description, imageUrl, utcNow);

        direction.Raise(new RecipeDirectionCreatedDomainEvent(direction.Id, recipeId));
        return direction;
    }
    public void Update(string description, DateTime utcNow)
    {
        Description = description;
        UpdatedOnUtc = utcNow;

        Raise(new RecipeDirectionUpdatedDomainEvent(Id));
    }
    public void UpdateImageUrl(Uri? imageUrl, DateTime utcNow)
    {
        ImageUrl = imageUrl;
        UpdatedOnUtc = utcNow;
    }
}
