using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Identity;

namespace Pento.Application.Users.SignIn;

public sealed record MobileSignInCommand(string Email, string Password)
    : ICommand<AuthToken>;
