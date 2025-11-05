namespace Pento.Application.FoodReferences.Get;

public sealed class PagedFoodReferencesResponse
{
    public IReadOnlyList<FoodReferenceResponse> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}
