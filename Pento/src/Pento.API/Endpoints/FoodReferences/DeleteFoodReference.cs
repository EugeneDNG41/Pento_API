using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class DeleteFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("food-references/{id:guid}",
            async (Guid id, ICommandHandler<DeleteFoodReferenceCommand> handler, CancellationToken ct) =>
            {
                var command = new DeleteFoodReferenceCommand(id);
                Result result = await handler.Handle(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : CustomResults.Problem(result
                );
            })
            .WithTags(Tags.FoodReferences);
    }
}
