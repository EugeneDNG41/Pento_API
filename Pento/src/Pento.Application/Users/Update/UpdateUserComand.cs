using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.Update;

public sealed record UpdateUserCommand(string FirstName, string LastName) : ICommand;
