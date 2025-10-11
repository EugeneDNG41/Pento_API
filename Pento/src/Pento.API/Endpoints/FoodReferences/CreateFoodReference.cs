using MediatR;
using Pento.API.Extensions;
using Pento.Application.FoodReferences.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class CreateFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateFoodReferenceCommand(
                request.Name,
                request.FoodGroup,
                request.Barcode,
                request.Brand,
                request.TypicalShelfLifeDays,
                request.OpenFoodFactsId,
                request.UsdaId
            );

            Result<Guid> result = await sender.Send(command, cancellationToken);

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
        public string? Barcode { get; init; }
        public string? Brand { get; init; }
        public int TypicalShelfLifeDays { get; init; }
        public string? OpenFoodFactsId { get; init; }
        public string? UsdaId { get; init; }
    }
}
