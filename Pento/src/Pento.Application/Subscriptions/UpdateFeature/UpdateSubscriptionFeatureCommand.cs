using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.UpdateFeature;

public sealed record UpdateSubscriptionFeatureCommand(
    Guid Id,
    string? FeatureCode,
    int? Quota,
    TimeUnit? ResetPeriod) : ICommand;


