using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.Get;

public sealed record GetFoodReferenceQuery(Guid FoodReferenceId) : IQuery<FoodReferenceResponse>;
