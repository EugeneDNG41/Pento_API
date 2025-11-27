using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Subscriptions.Create;

public sealed record UpdateSubscriptionCommand(Guid Id, string? Name, string? Description, bool? IsActive) : ICommand;


