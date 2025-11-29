using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.CancelUserSubscription;

public sealed record CancelUserSubscriptionCommand(Guid UserSubscriptionId, string? Reason) : ICommand;

