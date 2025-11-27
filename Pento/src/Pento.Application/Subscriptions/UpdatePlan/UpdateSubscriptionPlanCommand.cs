using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Subscriptions.UpdatePlan;

public sealed record UpdateSubscriptionPlanCommand(
    Guid Id,
    long? Amount,
    Currency? Currency,
    int? DurationInDays) : ICommand;


