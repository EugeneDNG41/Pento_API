using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.Enrich;

public sealed record EnrichFoodReferenceShelfLifeCommand(Guid FoodReferenceId)
    : ICommand<FoodEnrichmentResult>;
