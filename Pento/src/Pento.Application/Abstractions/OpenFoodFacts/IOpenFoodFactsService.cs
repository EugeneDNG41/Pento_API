using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.OpenFoodFacts;

public interface IOpenFoodFactsService
{
    Task<Result<Product>> FetchProductAsync(string code, CancellationToken cancellationToken);
}
public static class OpenFoodFactsErrors
{
    public static readonly Error OpenFoodFactsApiError = Error.Failure(
        "OpenFoodFacts.ApiError",
        "An error occurred while communicating with the OpenFoodFacts API."
    );
    public static readonly Error ProductNotFound = Error.Failure(
        "OpenFoodFacts.ProductNotFound",
        "The requested product was not found in the OpenFoodFacts database."
    );
}
