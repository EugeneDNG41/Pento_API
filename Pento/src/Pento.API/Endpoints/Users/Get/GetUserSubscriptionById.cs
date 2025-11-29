using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentSubscriptionById;
using Pento.Application.Users.GetUserSubscriptionById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetUserSubscriptionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/users/subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            IQueryHandler<GetUserSubscriptionByIdQuery, UserSubscriptionDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<UserSubscriptionDetailResponse> result = await handler.Handle(
                new GetUserSubscriptionByIdQuery(subscriptionId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(Permissions.ManageSubscriptions)
        .WithSummary("Get a specific user subscription.")
        .WithTags(Tags.Admin);
    }
}
