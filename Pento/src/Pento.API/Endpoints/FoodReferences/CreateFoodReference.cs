using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class CreateFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references", async (Request request, ICommandHandler<CreateFoodReferenceCommand, Guid> handler, CancellationToken cancellationToken) =>
        {
            if (!Enum.TryParse<FoodGroup>(request.FoodGroup, true, out FoodGroup foodGroup))
            {
                return Results.BadRequest($"Invalid food group: {request.FoodGroup}");
            }
            if (!Enum.TryParse<UnitType>(request.UnitType, true, out UnitType unitType))
            {
                return Results.BadRequest($"Invalid data type: {request.UnitType}");
            }
            var command = new CreateFoodReferenceCommand(
                request.Name,
                foodGroup,
                request.FoodCategoryId,
                request.Brand,
                request.Barcode,
                request.UsdaId,
                request.TypicalShelfLifeDays_Pantry,
                request.TypicalShelfLifeDays_Fridge,
                request.TypicalShelfLifeDays_Freezer,
                request.AddedBy,
                request.ImageUrl,
                unitType

            );


            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/food-references/{id}", id),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.FoodReferences);
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public string FoodGroup { get; init; } = string.Empty; 
        public string? Notes { get; init; }
        public int? FoodCategoryId { get; init; }
        public string? Brand { get; init; }
        public string? Barcode { get; init; }
        public string UsdaId { get; init; } = string.Empty;
        public int? TypicalShelfLifeDays_Pantry { get; init; } = 0;
        public int? TypicalShelfLifeDays_Fridge { get; init; } = 0;
        public int? TypicalShelfLifeDays_Freezer { get; init; } = 0;
        public Guid? AddedBy { get; init; }
        public Uri? ImageUrl { get; init; }
        public string? UnitType { get; init; }
        }
}
