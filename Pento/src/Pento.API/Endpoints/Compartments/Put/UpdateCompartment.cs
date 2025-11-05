using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Compartments.Delete;

internal sealed class UpdateCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("compartments/{compartmentId:guid}", async (
            Guid compartmentId,
            Request request,
            ICommandHandler<UpdateCompartmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new UpdateCompartmentCommand(compartmentId, request.Name, request.Notes), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Compartments)
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Name { get; init; }     
        public string? Notes { get; init; }
    }

}
