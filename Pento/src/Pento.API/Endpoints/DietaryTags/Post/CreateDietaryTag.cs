using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.DietaryTags.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.DietaryTags.Post;

internal sealed class CreateDietaryTag : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dietary-tags", async (
            Request request,
            ICommandHandler<CreateDietaryTagCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateDietaryTagCommand(
                request.Name,
                request.Description
            );

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
