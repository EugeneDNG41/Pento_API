using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.DeletePlan;

public sealed record DeleteSubscriptionPlanCommand(Guid Id) : ICommand;
