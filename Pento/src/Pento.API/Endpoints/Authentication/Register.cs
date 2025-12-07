using Pento.API.Extensions;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Register;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Authentication;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
            HttpContext context,
            Request request,
            ICommandHandler<RegisterUserCommand, AuthToken> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(new RegisterUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Authentication);
    }

    internal sealed class Request
    {
        public string Email { get; init; }

        public string Password { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}
