
using Microsoft.AspNetCore.Authentication;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.SignOut;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Post;

internal sealed class SignOut : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/sign-out", async (
            HttpContext context,
            ICommandHandler<SignOutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            string? accessToken = await context.GetTokenAsync("access_token");
            Result result1 = await handler.Handle(new SignOutCommand(accessToken), cancellationToken);
            return result1.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
