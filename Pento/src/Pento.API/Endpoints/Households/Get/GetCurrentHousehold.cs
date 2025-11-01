
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.GetCurrent;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Get;

internal sealed class GetCurrentHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("households/current", async (
            IQueryHandler<GetCurrentHouseholdQuery, HouseholdDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<HouseholdDetailResponse> result = await handler.Handle(
                new GetCurrentHouseholdQuery(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Households)
        .RequireAuthorization()
        .WithName(RouteNames.GetCurrentHousehold);
    }
}
