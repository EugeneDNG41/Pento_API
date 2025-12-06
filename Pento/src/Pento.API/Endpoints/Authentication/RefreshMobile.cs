using Pento.API.Extensions;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.RefreshToken;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Authentication;

internal sealed class RefreshMobile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/mobile-refresh", async (
            Request request,
            HttpContext context,
            ICommandHandler<RefreshTokenCommand, AuthToken> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(
                new RefreshTokenCommand(request.RefreshToken),
                cancellationToken);
            
            return result.Match(Results.Ok, CustomResults.Problem);
        }).AllowAnonymous().WithTags(Tags.Authentication).WithSummary("Refresh user authentication token");
    }
    internal sealed class Request
    {
        public string RefreshToken { get; init; }
    }
}
