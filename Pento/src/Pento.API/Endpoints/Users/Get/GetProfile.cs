using System.Security.Claims;
using MailKit.Search;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Application.Users.GetCurrentEntitlements;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/profile", async (ClaimsPrincipal claims, IQueryHandler<GetUserQuery, UserResponse> handler, CancellationToken cancellationToken) =>
        {
            Result<UserResponse> result = await handler.Handle(new GetUserQuery(claims.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}

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
        .WithDescription("Get entitlements for the current user.")
        .WithTags(Tags.Users);
    }
}
