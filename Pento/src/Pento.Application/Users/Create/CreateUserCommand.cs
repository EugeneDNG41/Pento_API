using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;

namespace Pento.Application.Users.Create;

public sealed record CreateUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<UserResponse>;

