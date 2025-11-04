using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JasperFx.Events;
using Marten.Events.Projections;
using Pento.Application.Abstractions.Data;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.FoodItems.Projections;

public record FoodItemHistoryEntry
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid FoodItemId { get; init; }
    public DateTime Timestamp { get; init; }
    public string? UserId { get; init; }
    public string Description { get; init; } = null!;
}
public class FoodItemHistoryTransformation(IGenericRepository<Unit> unitRepository) : EventProjection
{
    public async Task<FoodItemHistoryEntry> Create(IEvent<FoodItemAdded> @event)
    {
        Unit unit = await unitRepository.GetByIdAsync(@event.Data.UnitId);
        return new FoodItemHistoryEntry
        {
            FoodItemId = @event.Data.Id,
            Timestamp = @event.Timestamp.UtcDateTime,
            UserId = @event.UserName,
            Description = $"{@event.Data.Quantity} {unit!.Abbreviation} {@event.Data.Name} Added."
        };
    }
}
