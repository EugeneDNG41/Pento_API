using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.ResumeSubscription;

public sealed record ResumeSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

