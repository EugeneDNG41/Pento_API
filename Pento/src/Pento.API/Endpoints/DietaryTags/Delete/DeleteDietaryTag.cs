using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.DietaryTags.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.DietaryTags.Delete;

internal sealed class DeleteDietaryTag : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("dietary-tags/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteDietaryTagCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteDietaryTagCommand(id);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.DietaryTags);
    }
}
