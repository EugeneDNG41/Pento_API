using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Activities.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;

namespace Pento.API.Endpoints.Activity.Get;

internal sealed class GetActivities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/activities", async (
            string? searchText,
            ActivityType? type,
            IQueryHandler<GetActivitiesQuery, IReadOnlyList<ActivityResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<ActivityResponse>> result = await handler.Handle(new GetActivitiesQuery(searchText, type), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManageMilestones).WithTags(Tags.Admin);
    }
}
