using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid UserSubscriptionId, string? Reason) : ICommand;

