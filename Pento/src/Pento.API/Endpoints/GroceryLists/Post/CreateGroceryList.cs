using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryLists.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.GroceryLists.Post;

internal sealed class CreateGroceryList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("grocery-lists", async (
            Request request,
            ICommandHandler<CreateGroceryListCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateGroceryListCommand(
                request.Name
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryLists);
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
    }
}
