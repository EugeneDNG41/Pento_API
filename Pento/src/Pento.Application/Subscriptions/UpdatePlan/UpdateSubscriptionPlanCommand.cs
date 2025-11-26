using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.UpdatePlan;

public sealed record UpdateSubscriptionPlanCommand(
    Guid Id,
    long? PriceAmount,
    string? PriceCurrency,
    int? DurationValue,
    TimeUnit? DurationUnit) : ICommand;


