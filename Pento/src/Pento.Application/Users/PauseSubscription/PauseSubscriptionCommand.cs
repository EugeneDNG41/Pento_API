using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.PauseSubscription;

public sealed record PauseSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

