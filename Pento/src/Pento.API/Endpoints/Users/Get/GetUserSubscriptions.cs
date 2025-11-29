using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentSubscriptions;
using Pento.Application.Users.GetUserSubscriptions;
using Pento.Domain.Abstractions;
using Pento.Domain.UserSubscriptions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetUserSubscriptions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/users/{userId:guid}/subscriptions", async (
            Guid userId,
            string? searchText,
            SubscriptionStatus? status,
            int? fromDuration,
            int? toDuration,
            IQueryHandler <GetUserSubscriptionsQuery, IReadOnlyList<UserSubscriptionResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<UserSubscriptionResponse>> result = await handler.Handle(
                new GetUserSubscriptionsQuery(userId, searchText, status, fromDuration, toDuration), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithSummary("Get subscriptions for a specific user.")
        .WithTags(Tags.Admin);
    }
}
