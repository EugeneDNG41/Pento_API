using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodReferences.Enrich;
public interface IFoodAiEnricher
{
    Task<FoodEnrichmentResult> EnrichAsync(FoodEnrichmentAsk ask, CancellationToken ct);
}

public sealed record FoodEnrichmentAsk(
    string Name,
    string FoodGroup,       
    string DataType        
);


public sealed record FoodEnrichmentResult(
    string? ShortName,        
    int? SuggestedExpiryDays, 
    Uri? ImageUrl          
);
