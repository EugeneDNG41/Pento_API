using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Trades;

namespace Pento.Application.Abstractions.Messaging;

public sealed class MessageHub()  : Hub<IMessageClient>
{
    public async Task AddToHousehold(Guid householdId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, householdId.ToString());
    }
    public async Task RemoveFromHousehold(Guid householdId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, householdId.ToString());
    }
    public async Task AddToSession(Guid sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
    }
    public async Task RemoveFromSession(Guid sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
    }
#pragma warning disable S125 // client already call above
    //public override async Task OnConnectedAsync()
    //{
    //    Guid? householdId = userContext.HouseholdId;
    //    if (householdId != null)
    //    {
    //        await Groups.AddToGroupAsync(Context.ConnectionId, householdId.Value.ToString());
    //        IEnumerable<TradeSession> sessions = await tradeSessionRepository
    //            .FindAsync(ts => ts.OfferHouseholdId == householdId || ts.RequestHouseholdId == householdId);
    //        foreach (TradeSession session in sessions)
    //        {
    //            await Groups.AddToGroupAsync(Context.ConnectionId, session.Id.ToString());
    //        }
    //    }

    //    await base.OnConnectedAsync();
    //}
}
