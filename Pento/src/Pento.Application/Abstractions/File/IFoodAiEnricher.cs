using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.FoodReferences.Enrich;

namespace Pento.Application.Abstractions.File;
public interface IFoodAiEnricher
{
    Task<FoodEnrichmentResult> EnrichAsync(FoodEnrichmentAsk ask, CancellationToken ct);
}

public sealed record FoodEnrichmentAsk(
    string Name,
    string FoodGroup       
);




