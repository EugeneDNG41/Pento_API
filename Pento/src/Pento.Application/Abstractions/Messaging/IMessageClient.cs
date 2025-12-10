using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.FoodItems.Delete;
using Pento.Application.FoodItems.Search;
using Pento.Application.FoodItems.Update;

namespace Pento.Application.Abstractions.Messaging;

public interface IMessageClient
{
    Task ReceiveNotification(string content);
    Task FoodItemAdded(FoodItemPreview foodItem);
    Task FoodItemUpdated(UpdateFoodItemCommand command);
    Task FoodItemDeleted(DeleteFoodItemCommand command);
    Task FoodItemConsume(Guid foodItemId, decimal quantity);
}
public sealed class MessageHub(IUserContext userContext) : Hub<IMessageClient>
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
    public override async Task OnConnectedAsync()
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, householdId.Value.ToString());
        }

        await base.OnConnectedAsync();
    }
}
