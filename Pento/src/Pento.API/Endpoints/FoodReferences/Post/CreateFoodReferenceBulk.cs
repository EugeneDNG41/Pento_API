using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Create;
using Pento.Application.FoodReferences.Create.Bluk;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.API.Endpoints.FoodReferences.Post;

internal sealed class CreateFoodReferenceBulk : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/bulk", async (
            List<CreateFoodReferenceBulkRequest> requests,
            ICommandHandler<CreateFoodReferenceBulkCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var commandList = new List<CreateFoodReferenceCommand>();

            foreach (CreateFoodReferenceBulkRequest req in requests)
            {
                if (!Enum.TryParse<FoodGroup>(req.FoodGroup, true, out FoodGroup foodGroup))
                {
                    return Results.BadRequest(
                        $"Invalid food group for item '{req.Name}': {req.FoodGroup}"
                    );
                }

                if (!Enum.TryParse<UnitType>(req.UnitType, true, out UnitType unitType))
                {
                    return Results.BadRequest(
                        $"Invalid unit type for item '{req.Name}': {req.UnitType}"
                    );
                }

                Uri? imageUrl = null;
                if (!string.IsNullOrWhiteSpace(req.ImageUrl))
                {
                    imageUrl = new Uri(req.ImageUrl);
                }

                var cmd = new CreateFoodReferenceCommand(
                    req.Name,
                    foodGroup,
                    req.FoodCategoryId,
                    req.Brand,
                    req.Barcode,
                    req.UsdaId,
                    req.TypicalShelfLifeDays_Pantry,
                    req.TypicalShelfLifeDays_Fridge,
                    req.TypicalShelfLifeDays_Freezer,
                    req.AddedBy,
                    imageUrl,
                    unitType
                );

                commandList.Add(cmd);
            }

            Domain.Abstractions.Result result = await handler.Handle(
                new CreateFoodReferenceBulkCommand(commandList),
                cancellationToken
            );

            return result.Match(
                Results.NoContent,
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.FoodReferences);
    }
}
