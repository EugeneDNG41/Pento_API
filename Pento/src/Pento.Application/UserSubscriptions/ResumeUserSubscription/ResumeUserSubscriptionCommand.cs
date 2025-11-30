using Azure.Core;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserSubscriptions.ResumeUserSubscription;

public sealed record ResumeUserSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

