using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryLists.Create;
using Pento.Domain.Abstractions;

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
        .WithTags(Tags.GroceryLists)
                    .RequireAuthorization();


        app.MapPost("grocery-lists/detail", async (
                Request request,
                ICommandHandler<CreateGroceryListDetailCommand, Guid> handler,
                CancellationToken cancellationToken
            ) =>
                    {
                        var command = new CreateGroceryListDetailCommand(
                            request.Name,
                            request.Items
                        );

                        Result<Guid> result = await handler.Handle(command, cancellationToken);

                        return result.Match(
                            id => Results.Ok(new { Id = id }),
                            CustomResults.Problem
                        );
                    })
            .WithTags(Tags.GroceryLists)
            .WithDescription("priority: High, Medium, Low")
            .RequireAuthorization();
    }



    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public List<GroceryListItemRequest> Items { get; init; } = new();

    }
}
