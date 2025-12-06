using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.SignIn;

public sealed record MobileSignInCommand(string Email, string Password)
    : ICommand<AuthToken>;
