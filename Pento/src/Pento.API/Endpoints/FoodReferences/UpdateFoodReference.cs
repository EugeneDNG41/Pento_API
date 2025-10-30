using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class UpdateFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("food-references/{id:guid}",
            async (Guid id, Request request, ICommandHandler<UpdateFoodReferenceCommand, Guid> handler, CancellationToken ct) =>
            {
                var command = new UpdateFoodReferenceCommand(
                    Id: id,
                    Name: request.Name,
                    FoodGroup: request.FoodGroup,
                    DataType: request.DataType,
                    Notes: request.Notes,
                    FoodCategoryId: request.FoodCategoryId,
                    Brand: request.Brand,
                    Barcode: request.Barcode,
                    UsdaId: request.UsdaId,
                    PublishedOnUtc: request.PublishedOnUtc,
                    TypicalShelfLifeDays_Pantry: request.TypicalShelfLifeDays_Pantry,
                    TypicalShelfLifeDays_Fridge: request.TypicalShelfLifeDays_Fridge,
                    TypicalShelfLifeDays_Freezer: request.TypicalShelfLifeDays_Freezer,
                    ImageUrl: request.ImageUrl
                );

                Result<Guid> result = await handler.Handle(command, ct);

                return result.IsSuccess
                    ? Results.Ok(id)
                    : CustomResults.Problem(result
                );
            })
            .WithTags(Tags.FoodReferences);
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public string FoodGroup { get; init; } = string.Empty;
        public string DataType { get; init; } = string.Empty;
        public string? Notes { get; init; }
        public int? FoodCategoryId { get; init; }
        public string? Brand { get; init; }
        public string? Barcode { get; init; }
        public string UsdaId { get; init; } = string.Empty;
        public DateTime PublishedOnUtc { get; init; } = DateTime.UtcNow;
        public int? TypicalShelfLifeDays_Pantry { get; init; }
        public int? TypicalShelfLifeDays_Fridge { get; init; }
        public int? TypicalShelfLifeDays_Freezer { get; init; }
        public Uri? ImageUrl { get; init; }
    }
}
