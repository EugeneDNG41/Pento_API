
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.RefreshToken;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Post;

internal sealed class Refresh : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh", async (
            HttpContext context,
            ICommandHandler<RefreshTokenCommand, AuthToken> handler,
            CancellationToken cancellationToken) =>
        {
            context.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);
            Result<AuthToken> result = await handler.Handle(
                new RefreshTokenCommand(refreshToken),
                cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).AllowAnonymous().WithTags(Tags.Users).WithSummary("Refresh user authentication token");
    }
}
