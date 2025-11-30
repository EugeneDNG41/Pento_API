using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.CancelUserSubscription;

public sealed record CancelUserSubscriptionCommand(Guid UserSubscriptionId, string? Reason) : ICommand;

