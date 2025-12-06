using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Activities.GetAll;
using Pento.Application.UserActivities.GetCurrentActivities;
using Pento.Application.UserActivities.GetUserActivities;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Activity.Get;

internal sealed class GetActivities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/activities", async (
            string? searchText,
            IQueryHandler<GetActivitiesQuery, IReadOnlyList<ActivityResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<ActivityResponse>> result = await handler.Handle(new GetActivitiesQuery(searchText), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManageUsers).WithTags(Tags.Admin);
    }
}
internal sealed class GetUserActivities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/users/activities", async (
            Guid[]? userIds,
            string[]? activityCodes,
            string? keyword,
            DateTime? from,
            DateTime? to,
            GetUserActivitiesSortBy? sort,
            SortOrder? order,
            IQueryHandler<GetUserActivitiesQuery, PagedList<UserActivityResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<UserActivityResponse>> result = await handler.Handle(
                new GetUserActivitiesQuery(userIds, activityCodes, keyword, from, to, sort, order, pageNumber, pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
}
