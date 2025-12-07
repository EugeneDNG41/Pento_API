using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.GenerateInvite;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Post;

internal sealed class GenerateInviteCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("households/invites", async (
            Request request,
            ICommandHandler<GenerateInviteCodeCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await handler.Handle(
                new GenerateInviteCodeCommand(request.CodeExpirationDate), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Households)
        .RequireAuthorization(Permissions.ManageHousehold);
    }
    internal sealed class Request
    {
        public DateTime? CodeExpirationDate { get; init; }
    }
}
