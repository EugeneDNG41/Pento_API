using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Activities.GetSummary;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetActivitySummary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/activities/summary", async (
            string[]? codes,
            Guid[]? userIds,
            Guid[]? householdIds,
            DateOnly? fromDate,
            DateOnly? toDate,
            TimeWindow? timeWindow,
            IQueryHandler<GetActivitySummaryQuery, IReadOnlyList<ActivitySummary>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetActivitySummaryQuery(codes, userIds, householdIds, fromDate, toDate, timeWindow);
            Result<IReadOnlyList<ActivitySummary>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManageUsers).WithTags(Tags.Admin);
    }
}
