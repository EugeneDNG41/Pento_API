using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.DeviceTokens;

namespace Pento.Application.Notifications.DeviceTokens;
public sealed record RegisterDeviceTokenCommand(
    string Token,
    DevicePlatform Platform
) : ICommand<string>;
