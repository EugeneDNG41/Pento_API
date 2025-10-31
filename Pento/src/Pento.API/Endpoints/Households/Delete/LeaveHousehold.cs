using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Leave;
using Pento.Application.Households.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.API.Endpoints.Households.Delete;

internal sealed class LeaveHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("households/leave", async (
            IUserContext userContext,
            ICommandHandler<LeaveHouseholdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new LeaveHouseholdCommand(userContext.UserId, userContext.HouseholdId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
}
