using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Compartments.Post;

internal sealed class CreateCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("storages/{storageId:guid}/compartments", async (
            Guid storageId,
            Request request,
            ICommandHandler<CreateCompartmentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(
                new CreateCompartmentCommand(storageId, request.Name, request.Notes), cancellationToken);
            return result
            .Match(id => Results.Ok(new { compartmentId = id }), CustomResults.Problem);
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
