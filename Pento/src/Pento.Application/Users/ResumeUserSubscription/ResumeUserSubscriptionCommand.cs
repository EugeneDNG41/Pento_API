using Azure.Core;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.ResumeUserSubscription;

public sealed record ResumeUserSubscriptionCommand(Guid UserSubscriptionId) : ICommand;

