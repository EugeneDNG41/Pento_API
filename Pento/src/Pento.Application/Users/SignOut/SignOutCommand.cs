using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.SignOut;

public sealed record SignOutCommand(string? AccessToken) : ICommand;
