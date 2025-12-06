using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Activities;

namespace Pento.Application.UserActivities.GetUserActivities;

public sealed record GetUserActivitiesQuery(Guid? UserId, string? SearchTerm, DateTime? FromDate, DateTime? ToDate, int PageNumber, int PageSize) : IQuery<PagedList<UserActivityResponse>>;
