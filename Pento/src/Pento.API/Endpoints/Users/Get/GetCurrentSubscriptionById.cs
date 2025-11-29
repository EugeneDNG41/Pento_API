using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentSubscriptionById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetCurrentSubscriptionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            IQueryHandler <GetCurrentSubscriptionByIdQuery, UserSubscriptionDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<UserSubscriptionDetailResponse> result = await handler.Handle(
                new GetCurrentSubscriptionByIdQuery(subscriptionId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithSummary("Get a specific subscription for the current user.")
        .WithTags(Tags.Users);
    }
}
