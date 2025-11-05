
namespace Pento.Application.RecipeDirections.GetAll;

public sealed class RecipeWithDirectionsResponse
{
    public Guid RecipeId { get; init; }
    public string RecipeTitle { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<RecipeDirectionItemResponse> Directions { get; init; } = new();
}

public sealed class RecipeDirectionItemResponse
{
    public Guid DirectionId { get; init; }
    public int StepNumber { get; init; }
    public string Description { get; init; } = string.Empty;
    public Uri? ImageUrl { get; init; }
}
