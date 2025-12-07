using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.GetCurrentSubscriptions;
using Pento.Domain.Abstractions;
using Pento.Domain.UserSubscriptions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetCurrentSubscriptions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/subscriptions", async (
            string? searchText,
            SubscriptionStatus? status,
            int? fromDuration,
            int? toDuration,
            IQueryHandler<GetCurrentSubscriptionsQuery, IReadOnlyList<UserSubscriptionResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<UserSubscriptionResponse>> result = await handler.Handle(
                new GetCurrentSubscriptionsQuery(searchText, status, fromDuration, toDuration), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithSummary("Get subscriptions for the current user.")
        .WithTags(Tags.Users);
    }
}
