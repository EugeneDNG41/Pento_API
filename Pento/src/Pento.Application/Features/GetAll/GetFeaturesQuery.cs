using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Features.GetAll;

public sealed record GetFeaturesQuery(string? SearchText) : IQuery<IReadOnlyList<FeatureResponse>>;
