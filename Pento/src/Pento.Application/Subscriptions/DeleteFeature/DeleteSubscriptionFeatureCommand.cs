using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.DeleteFeature;

public sealed record DeleteSubscriptionFeatureCommand(Guid Id) : ICommand;
