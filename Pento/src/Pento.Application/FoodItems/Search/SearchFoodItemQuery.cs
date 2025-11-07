using Marten.Pagination;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodItems.Search;

public sealed record SearchFoodItemQuery(
    string? SearchText,
    string? FoodGroups,
    decimal? FromQuantity,
    decimal? ToQuantity,
    DateTime? ExpirationDateAfter,
    DateTime? ExpirationDateBefore,
    int PageNumber,
    int PageSize) : IQuery<IPagedList<FoodItemPreview>>;
