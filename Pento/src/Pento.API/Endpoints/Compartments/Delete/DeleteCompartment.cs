using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Delete;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Compartments.Delete;

internal sealed class DeleteCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("compartments/{compartmentId:guid}", async (
            Guid compartmentId,
            ICommandHandler<DeleteCompartmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new DeleteCompartmentCommand(compartmentId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Compartments)
        .RequireAuthorization();
    }
}
