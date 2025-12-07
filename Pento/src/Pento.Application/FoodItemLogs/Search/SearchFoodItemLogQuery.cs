using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.FoodItemLogs;

namespace Pento.Application.FoodItemLogs.Search;

public sealed record SearchFoodItemLogQuery(
    string? Name,
    DateTime? FromUtc,
    DateTime? ToUtc,
    FoodItemLogAction? LogAction,
    int PageNumber,
    int PageSize) : IQuery<PagedList<FoodItemLogPreview>>;
