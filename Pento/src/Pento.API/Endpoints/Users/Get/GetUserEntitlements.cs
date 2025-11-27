using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentEntitlements;
using Pento.Application.Users.GetUserEntitlements;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetUserEntitlements : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}/entitlements", async (
            Guid userId,
            string ? searchText,
            bool ? available,
            IQueryHandler <GetUserEntitlementsQuery, IReadOnlyList<UserEntitlementResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<UserEntitlementResponse>> result = await handler.Handle(
                new GetUserEntitlementsQuery(userId, searchText, available), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithSummary("Get entitlements for a specific user by their user Code (admin only).")
        .WithTags(Tags.Users);
    }
}
