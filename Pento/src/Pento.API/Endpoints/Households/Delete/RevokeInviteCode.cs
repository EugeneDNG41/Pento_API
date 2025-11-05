using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.RemoveMember;
using Pento.Application.Households.RevokeInvite;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Households.Delete;

internal sealed class RevokeInviteCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("households/invites", async (
            ICommandHandler<RevokeInviteCodeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new RevokeInviteCodeCommand(), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
}
