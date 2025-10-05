using System.Security.Claims;
using MediatR;
using Pento.API.Extensions;
using Pento.Application.Users.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Users;

internal sealed class GetProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/profile", async (ClaimsPrincipal claims, ISender sender, CancellationToken cancellationToken) =>
        {
            Result<UserResponse> result = await sender.Send(new GetUserQuery(claims.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(Permission.GetUser.Code)
        .WithTags(Tags.Users);
    }
}
