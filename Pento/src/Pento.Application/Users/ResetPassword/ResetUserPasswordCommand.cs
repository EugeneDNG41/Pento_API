using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.ResetPassword;

public sealed record ResetUserPasswordCommand(string Email) : ICommand;
