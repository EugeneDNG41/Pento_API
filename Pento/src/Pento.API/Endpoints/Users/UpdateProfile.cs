using System.Security.Claims;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Users;

internal sealed class UpdateProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile", async (Request request,  ClaimsPrincipal claims, ICommandHandler<UpdateUserCommand> handler, CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateUserCommand(
                claims.GetUserId(),
                request.FirstName,
                request.LastName), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }

    internal sealed class Request
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}
