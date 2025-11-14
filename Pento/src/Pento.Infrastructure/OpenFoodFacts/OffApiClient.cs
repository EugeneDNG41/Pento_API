using GenerativeAI;
using GenerativeAI.Types;
using OpenFoodFacts4Net.ApiClient;
using OpenFoodFacts4Net.Json.Data;
using Pento.Domain.Abstractions;


namespace Pento.Infrastructure.OpenFoodFacts;

public sealed class OffApiClient(GeminiModel model)
{
    public async static Task<Product> FetchProduct(string barcode)
    {
        string userAgent = UserAgentHelper.GetUserAgent("Pento", ".Net Platform", "1.0", null);
        var client = new Client(userAgent);
        GetProductResponse product = await client.GetProductAsync(barcode);
        return product.Product;
    } 
    public async Task<Result<string?>> TestAi(string prompt)
    {
        GenerateContentResponse response = await model.GenerateContentAsync(prompt);
        return response.Text();
    }
}
