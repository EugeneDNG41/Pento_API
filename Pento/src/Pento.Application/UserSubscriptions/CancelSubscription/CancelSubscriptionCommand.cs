using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid UserSubscriptionId, string? Reason) : ICommand;

