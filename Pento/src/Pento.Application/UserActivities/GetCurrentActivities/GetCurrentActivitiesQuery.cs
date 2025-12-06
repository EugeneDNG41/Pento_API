using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.UserActivities.GetCurrentActivities;

public sealed record GetCurrentActivitiesQuery(
    string[]? ActivityCodes,
    string? SearchTerm,
    DateTime? FromDate, 
    DateTime? ToDate,
    GetUserActivitiesSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber, 
    int PageSize) : IQuery<PagedList<CurrentUserActivityResponse>>;
public enum GetUserActivitiesSortBy
{
    Name,
    Description,
    PerformedOn
}
