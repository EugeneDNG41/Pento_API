using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.Create;

public sealed record CreateSubscriptionCommand(string Name, string Description, bool IsActive) : ICommand<Guid>;


