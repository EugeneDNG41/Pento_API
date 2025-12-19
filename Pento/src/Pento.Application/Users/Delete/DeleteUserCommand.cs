using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.Delete;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
