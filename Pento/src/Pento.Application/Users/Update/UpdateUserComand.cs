using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.Update;

public sealed record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand;
