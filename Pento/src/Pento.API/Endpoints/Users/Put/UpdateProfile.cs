using System.Security.Claims;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class UpdateProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile", async (Request request, ICommandHandler<UpdateUserCommand> handler, CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateUserCommand(
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
