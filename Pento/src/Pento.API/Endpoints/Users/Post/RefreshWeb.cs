
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.RefreshToken;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Post;

internal sealed class RefreshWeb : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/web-refresh", async (
            HttpContext context,
            ICommandHandler<RefreshTokenCommand, AuthToken> handler,
            CancellationToken cancellationToken) =>
        {
            context.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken);
            Result<AuthToken> result = await handler.Handle(
                new RefreshTokenCommand(refreshToken),
                cancellationToken);
            if (result.IsSuccess)
            {
                context.Response.Cookies.Append("refreshToken", result.Value.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            }
            return result.Match(token => Results.Ok(new { token.AccessToken }), CustomResults.Problem);
        }).AllowAnonymous().WithTags(Tags.Users).WithSummary("Refresh user authentication token");
    }
}
