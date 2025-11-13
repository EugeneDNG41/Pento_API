using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.DietaryTags.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.DietaryTags.Put;

internal sealed class UpdateDietaryTag : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("dietary-tags/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateDietaryTagCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateDietaryTagCommand(id, request.Name, request.Description);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.DietaryTags);
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
    }
}
