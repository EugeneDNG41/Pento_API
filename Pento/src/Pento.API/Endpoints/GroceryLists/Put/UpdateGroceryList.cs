using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryLists.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryLists.Put;

internal sealed class UpdateGroceryList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("grocery-lists/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateGroceryListCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateGroceryListCommand(id, request.Name);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                deletedId => Results.Ok(new { Id = deletedId }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryLists)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
    }
}
