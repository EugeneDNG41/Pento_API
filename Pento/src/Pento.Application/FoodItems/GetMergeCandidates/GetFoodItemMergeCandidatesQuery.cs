using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Search;

namespace Pento.Application.FoodItems.GetMergeCandidates;

public sealed record GetFoodItemMergeCandidatesQuery(Guid Id) : IQuery<IReadOnlyList<FoodItemPreview>>;

