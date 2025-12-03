using Microsoft.Extensions.Options;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Activities.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Infrastructure.ThirdPartyServices.PayOS;

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
        }).RequireAuthorization(Permissions.ManageMilestones).WithTags(Tags.Admin);
    }
}
internal sealed class TestJsonToString : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("test/json-to-string", (
            IOptions<PayOSCustomOptions> options) =>
        {
            return Results.Ok(options.Value.ToString());
        }).WithTags(Tags.Activities);
    }
}
