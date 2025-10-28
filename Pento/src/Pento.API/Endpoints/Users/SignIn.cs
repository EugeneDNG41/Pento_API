using Pento.API.Extensions;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.SignIn;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Users;

internal sealed class SignIn : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/sign-in", async (Request request, ICommandHandler<SignInUserCommand, AuthToken> handler, CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(new SignInUserCommand(
                request.Email,
                request.Password), cancellationToken);

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
