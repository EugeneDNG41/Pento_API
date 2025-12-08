using Microsoft.AspNetCore.SignalR;
using Pento.Infrastructure.Authentication;

namespace Pento.Infrastructure.Authorization;

internal sealed class HubUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.GetUserId().ToString();
    }
}
