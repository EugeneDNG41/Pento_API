using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.Search;

public sealed record SearchFoodItemQuery(
    Guid? FoodReferenceId,
    string? SearchText,
    FoodGroup[]? FoodGroup,
    FoodItemStatus[]? Status,
    decimal? FromQuantity,
    decimal? ToQuantity,
    DateOnly? ExpirationDateAfter,
    DateOnly? ExpirationDateBefore,
    FoodItemPreviewSortBy SortBy,
    SortOrder SortOrder,
    int PageNumber,
    int PageSize) : IQuery<PagedList<FoodItemPreview>>;
public enum FoodItemPreviewSortBy
{
    Default,
    Name,
    FoodGroup,
    Quantity,
    ExpirationDate
}
public enum FoodItemStatus
{
    Fresh,
    Expiring,
    Expired
}
