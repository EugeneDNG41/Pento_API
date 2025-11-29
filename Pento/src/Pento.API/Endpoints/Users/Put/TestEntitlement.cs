
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Entitlement;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class TestEntitlement : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/entitlements", async (
            FeatureCode featureCode,
            IUserContext userContext,
            IEntitlementService entitlementService,
            CancellationToken cancellationToken) =>
        {
            Result result = await entitlementService.CheckEntitlementAsync(userContext.UserId, featureCode, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }

}
