using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserEntitlements.GetCurrentEntitlements;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetCurrentEntitlements : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/entitlements", async (
            string ? searchText,
            bool ? available,
            IQueryHandler <GetCurrentEntitlementsQuery, IReadOnlyList<UserEntitlementResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<UserEntitlementResponse>> result = await handler.Handle(
                new GetCurrentEntitlementsQuery(searchText, available), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithSummary("Get entitlements for the current user.")
        .WithTags(Tags.Users);
    }
}
