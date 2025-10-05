﻿using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<Guid>;
