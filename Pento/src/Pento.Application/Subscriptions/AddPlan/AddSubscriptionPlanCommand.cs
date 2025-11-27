using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.AddPlan;

public sealed record AddSubscriptionPlanCommand(Guid SubscriptionId,
    long Amount,
    Currency Currency,
    int? DurationInDays) : ICommand<Guid>;


