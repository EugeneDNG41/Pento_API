using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Application.Users.Search;

namespace Pento.Application.Users.Create;

public sealed record CreateUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<BasicUserResponse>;

