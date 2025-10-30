using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodReferences.Enrich;
public sealed record FoodEnrichmentResult(
    Guid Id,
    int TypicalShelfLifeDays_Pantry,
    int TypicalShelfLifeDays_Fridge,
    int TypicalShelfLifeDays_Freezer
   );

