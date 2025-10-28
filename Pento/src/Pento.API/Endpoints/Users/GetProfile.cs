using System.Security.Claims;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Users;

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
