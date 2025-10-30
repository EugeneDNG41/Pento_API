using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Leave;
using Pento.Application.Households.RemoveMember;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.API.Endpoints.Households.Delete;

internal sealed class RemoveMember : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("households/{householdId:guid}/members/{userId:guid}", async (
            Guid householdId,
            Guid userId,
            IUserContext userContext,
            ICommandHandler<RemoveHouseholdMemberCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = userContext.HouseholdId != householdId
                ? Result.Failure(HouseholdErrors.NotFound)
                : await handler.Handle(
                new RemoveHouseholdMemberCommand(householdId, userId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization(policy => policy.RequireRole(Role.HouseholdAdmin.Name, Role.PowerMember.Name));
    }
}
