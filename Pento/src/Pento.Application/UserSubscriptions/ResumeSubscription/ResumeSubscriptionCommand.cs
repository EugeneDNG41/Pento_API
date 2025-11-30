using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.ResumeSubscription;

public sealed record ResumeSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

