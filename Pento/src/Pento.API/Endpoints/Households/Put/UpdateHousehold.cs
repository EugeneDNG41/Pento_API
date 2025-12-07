using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Put;

internal sealed class UpdateHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("households", async (
            Request request,
            ICommandHandler<UpdateHouseholdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new UpdateHouseholdCommand(request.Name), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Name { get; init; }
    }
}
