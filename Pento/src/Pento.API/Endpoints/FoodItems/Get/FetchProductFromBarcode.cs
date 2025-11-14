using OpenFoodFacts4Net.Json.Data;
using Pento.API.Extensions;

using Pento.Domain.Abstractions;
using Pento.Infrastructure.OpenFoodFacts;

namespace Pento.API.Endpoints.FoodItems.Get;

internal sealed class FetchProductFromBarcode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-items/barcode/{barcode}", async (
            string barcode,
            CancellationToken cancellationToken) =>
        {
            Result<Product> result = await OffApiClient.FetchProduct(barcode);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .WithDescription("Fetch product information from a barcode");
    }
}
internal sealed class AiTest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-items/ai-test", async (
            string prompt,
            OffApiClient client,
            CancellationToken cancellationToken) =>
        {
            Result<string?> result = await client.TestAi(prompt);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .WithDescription("AI Test Endpoint");
    }
}
