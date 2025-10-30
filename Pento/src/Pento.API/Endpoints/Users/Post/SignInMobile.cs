using Pento.API.Extensions;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.SignIn;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Users.Post;

internal sealed class SignInMobile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/mobile-sign-in", async (
            HttpContext context,
            Request request, 
            ICommandHandler<SignInUserCommand, AuthToken> handler, 
            CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(new SignInUserCommand(
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
        .WithTags(Tags.Users);
    }
    internal sealed class Request
    {
        public string Email { get; init; }

        public string Password { get; init; }
    }
}
