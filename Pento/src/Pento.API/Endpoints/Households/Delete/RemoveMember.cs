using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.RemoveMember;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Delete;

internal sealed class RemoveMember : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("households/members/{userId:guid}", async (
            Guid userId,
            ICommandHandler<RemoveHouseholdMemberCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new RemoveHouseholdMemberCommand(userId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
}
