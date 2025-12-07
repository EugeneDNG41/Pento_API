using Pento.Application.Abstractions.Messaging;
using Pento.Domain.DeviceTokens;

namespace Pento.Application.Notifications.DeviceTokens;

public sealed record RegisterDeviceTokenCommand(
    string Token,
    DevicePlatform Platform
) : ICommand<string>;
