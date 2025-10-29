
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
            Request request,
            ICommandHandler<RefreshTokenCommand, AuthToken> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(
                new RefreshTokenCommand(request.RefreshToken),
                cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).AllowAnonymous().WithTags(Tags.Users).WithSummary("Refresh user authentication token");
    }
    internal sealed class Request
    {
        public string RefreshToken { get; init; }
    }
}
