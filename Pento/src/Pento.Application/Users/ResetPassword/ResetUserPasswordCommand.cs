using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.ResetPassword;

public sealed record ResetUserPasswordCommand(string Email) : ICommand;
