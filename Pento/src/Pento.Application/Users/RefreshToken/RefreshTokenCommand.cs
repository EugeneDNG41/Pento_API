using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.RefreshToken;

public sealed record RefreshTokenCommand(string? RefreshToken) : ICommand<AuthToken>;
