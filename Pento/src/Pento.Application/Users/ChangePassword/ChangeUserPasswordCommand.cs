using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.ChangePassword;

public sealed record ChangeUserPasswordCommand(string CurrentPassword, string NewPassword, string ConfirmNewPassword) : ICommand;
