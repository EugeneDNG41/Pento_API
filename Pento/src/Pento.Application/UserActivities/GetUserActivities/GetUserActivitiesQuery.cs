using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.UserActivities.GetCurrentActivities;
using Pento.Domain.Activities;

namespace Pento.Application.UserActivities.GetUserActivities;

public sealed record GetUserActivitiesQuery(
    Guid[]? UserIds,
    string[]? ActivityCodes,
    string? SearchTerm, 
    DateTime? FromDate, 
    DateTime? ToDate,
    GetUserActivitiesSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber, 
    int PageSize) : IQuery<PagedList<UserActivityResponse>>;
