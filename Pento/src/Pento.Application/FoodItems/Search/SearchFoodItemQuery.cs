using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.Search;

public sealed record SearchFoodItemQuery(
    string? SearchText,
    FoodGroup? FoodGroup,
    decimal? FromQuantity,
    decimal? ToQuantity,
    DateOnly? ExpirationDateAfter,
    DateOnly? ExpirationDateBefore,
    FoodItemStatus? Status,
    int PageNumber,
    int PageSize) : IQuery<PagedList<FoodItemPreview>>;
