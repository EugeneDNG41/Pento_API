using OpenFoodFacts4Net.Json.Data;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.OpenFoodFacts;
using Pento.Application.FoodReferences.Get;
using Pento.Application.FoodReferences.Scan;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Infrastructure.OpenFoodFacts;

namespace Pento.API.Endpoints.FoodReferences.Get;

internal sealed class FetchProductFromBarcode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-references/{barcode}", async (
            string barcode,
            IQueryHandler<ScanBarcodeQuery, FoodReferenceResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<FoodReferenceResponse> result = await handler.Handle(new ScanBarcodeQuery(barcode), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences)
        .WithDescription("Fetch product information from a barcode");
    }
}

