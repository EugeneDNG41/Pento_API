using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.Abstractions.ThirdPartyServices.Barcode;

public interface IBarcodeService
{
    Task<Result<FoodReference>> FetchProductAsync(string barcode, CancellationToken cancellationToken);
}
public static class BarcodeServiceErrors
{
    public static readonly Error ApiError = Error.Failure(
        "BarcodeApiError.ApiError",
        "An error occurred while fetching the product from OpenFoodFacts."
    );
    public static readonly Error ProductNotFound = Error.Failure(
        "OpenFoodFacts.ProductNotFound",
        "The requested product was not found in the OpenFoodFacts database."
    );
}
