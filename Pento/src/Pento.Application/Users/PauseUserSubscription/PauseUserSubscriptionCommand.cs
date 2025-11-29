using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.PauseUserSubscription;

public sealed record PauseUserSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

