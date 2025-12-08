using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Messaging;
using Pento.Infrastructure.Authentication;

namespace Pento.Infrastructure.Services;

public sealed class MessageHub : Hub<IMessageClient>
{
    public async Task SendNotification(string content)
    {
        await Clients.All.ReceiveNotification(content);
    }
    public override async Task OnConnectedAsync()
    {
        Guid? householdId = Context.GetHttpContext()?.User.GetHouseholdId();
        if (householdId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, householdId.Value.ToString());
        }

        await base.OnConnectedAsync();
    }
}
