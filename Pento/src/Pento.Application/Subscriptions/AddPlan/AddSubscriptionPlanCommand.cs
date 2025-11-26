using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.AddPlan;

public sealed record AddSubscriptionPlanCommand(Guid SubscriptionId,
    long PriceAmount,
    string PriceCurrency,
    int? DurationValue,
    TimeUnit? DurationUnit) : ICommand<Guid>;


