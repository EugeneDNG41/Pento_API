using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Leave;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Delete;

internal sealed class LeaveHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("households/leave", async (
            ICommandHandler<LeaveHouseholdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new LeaveHouseholdCommand(), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
}
