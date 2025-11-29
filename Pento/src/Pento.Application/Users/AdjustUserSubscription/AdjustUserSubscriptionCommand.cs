using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.AdjustUserSubscription;

public sealed record AdjustUserSubscriptionCommand(Guid UserSubscriptionId, int DurationInDays) : ICommand;

