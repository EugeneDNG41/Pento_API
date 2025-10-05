using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.SignIn;

public sealed record SignInUserCommand(string Email, string Password)
    : ICommand<AuthToken>;
