using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Get;

public sealed record GetAllFoodReferencesQuery(
    FoodGroup[]? FoodGroup,
    string? Search,
    bool? HasImage,
    GetAllFoodReferencesSortBy? SortBy,
    SortOrder? SortOrder,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedList<FoodReferenceResponse>>;

public enum GetAllFoodReferencesSortBy
{
    Name,
    FoodGroup,
    Brand,
    CreatedAt
}
