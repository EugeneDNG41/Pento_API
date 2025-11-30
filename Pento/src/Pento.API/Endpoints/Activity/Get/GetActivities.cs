using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Activities.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Activity.Get;

internal sealed class GetActivities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("activities", async (
            string? searchText,
            IQueryHandler<GetActivitiesQuery, IReadOnlyList<ActivityResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<ActivityResponse>> result = await handler.Handle(new GetActivitiesQuery(searchText), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Activities);
    }
}
