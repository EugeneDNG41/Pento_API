using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.UserActivities.GetCurrentActivities;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetUserActivities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/activities", async (
            string[]? activityCodes,
            string? searchTerm,
            DateTime? fromDate,
            DateTime? toDate,
            GetUserActivitiesSortBy? sortBy,
            SortOrder? sortOrder,
            IQueryHandler<GetCurrentActivitiesQuery, PagedList<CurrentUserActivityResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetCurrentActivitiesQuery(
                activityCodes,
                searchTerm,
                fromDate,
                toDate,
                sortBy,
                sortOrder,
                pageNumber,
                pageSize);
            Result<PagedList<CurrentUserActivityResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
