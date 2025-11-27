using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.Delete;

public sealed record DeleteSubscriptionCommand(Guid Id) : ICommand;
