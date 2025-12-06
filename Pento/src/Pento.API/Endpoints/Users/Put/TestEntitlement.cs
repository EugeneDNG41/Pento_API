
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class TestEntitlement : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/entitlements/test", async (
            Request request,
            IUserContext userContext,
            IEntitlementService entitlementService,
            CancellationToken cancellationToken) =>
        {
            Result result = await entitlementService.UseEntitlementAsync(userContext.UserId, request.FeatureCode, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string FeatureCode { get; init; }
    }
}
