using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.AdjustUserSubscription;

public sealed record AdjustUserSubscriptionCommand(Guid UserSubscriptionId, int DurationInDays) : ICommand;

