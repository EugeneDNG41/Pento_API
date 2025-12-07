using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.SetRoles;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Put;

internal sealed class SetMemberRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("households/members/{memberId:guid}/roles", async (
            Guid memberId,
            Request request,
            ICommandHandler<SetMemberRolesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            SetMemberRolesCommand command = new(memberId, request.Roles);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
    internal sealed class Request
    {
        public List<string> Roles { get; init; }
    }
}

