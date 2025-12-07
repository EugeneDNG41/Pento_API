using Pento.API.Extensions;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.SignIn;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Authentication;

internal sealed class SignInMobile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/mobile-sign-in", async (
            HttpContext context,
            Request request,
            ICommandHandler<MobileSignInCommand, AuthToken> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(new MobileSignInCommand(
                request.Email,
                request.Password), cancellationToken);
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
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Authentication);
    }
    internal sealed class Request
    {
        public string Email { get; init; }

        public string Password { get; init; }
    }
}
