using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.OpenFoodFacts;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Infrastructure.OpenFoodFacts;

internal sealed class OpenFoodFactsService(HttpClient httpClient) : IOpenFoodFactsService
{
    public async Task<Result<Product>> FetchProductAsync(
        string code,
        CancellationToken cancellationToken)
    {
        try
        {            
            using HttpResponseMessage response = await httpClient.GetAsync(
                $"/product/{code}.json",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            ProductResponse? productResponse = await response.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: cancellationToken);
            if (productResponse is null)
            {
                return Result.Failure<Product>(OpenFoodFactsErrors.OpenFoodFactsApiError);
            }
            if (productResponse.Status is false)
            {
                return Result.Failure<Product>(OpenFoodFactsErrors.ProductNotFound);
            }
            return productResponse.Product;
        }
        catch (HttpRequestException)
        {
            return Result.Failure<Product>(OpenFoodFactsErrors.OpenFoodFactsApiError);
        }
    }
}
