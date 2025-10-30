using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.Enrich;
public sealed record EnrichFoodReferenceShelfLifeCommand(Guid FoodReferenceId)
    : ICommand<FoodEnrichmentResult>;
