using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Identity;

namespace Pento.Application.Users.RefreshToken;

public sealed record RefreshTokenCommand(string? RefreshToken) : ICommand<AuthToken>;
