using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.PauseUserSubscription;

public sealed record PauseUserSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

