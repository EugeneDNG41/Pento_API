using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Identity;

namespace Pento.Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<AuthToken>;

