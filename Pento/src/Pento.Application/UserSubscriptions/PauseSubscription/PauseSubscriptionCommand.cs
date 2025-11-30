using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.PauseSubscription;

public sealed record PauseSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

