using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.FoodReferences.Enrich;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.External.File;
public interface IFoodAiEnricher
{
    Task<FoodEnrichmentResult> EnrichAsync(FoodEnrichmentAsk ask, CancellationToken ct);
    Task<Result<ProductExtraInformationWithoutFoodGroup>> EnrichAsync(string foodName, CancellationToken cancellationToken);
}

public sealed record FoodEnrichmentAsk(
    string Name,
    string FoodGroup       
);

public sealed record ProductExtraInformationWithoutFoodGroup(int UnitType, int PantryShelfLife, int FridgeShelfLife, int FreezerShelfLife);

public static class FoodAiEnricherErrors
{
    public static readonly Error ApiError = Error.Failure(
        "FoodAiEnricher.ApiError",
        "An error occurred while communicating with the AI service.");
}
