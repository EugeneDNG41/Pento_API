using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JasperFx.Events;
using Marten.Events.Projections;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Projections;

public record FoodItemHistoryEntry
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid FoodItemId { get; init; }
    public DateTime Timestamp { get; init; }
    public string? UserId { get; init; }
    public string Description { get; init; } = null!;
}
public class FoodItemHistoryTransformation() : EventProjection
{
    public FoodItemHistoryEntry Create(IEvent<FoodItemAdded> e)
    {
        return new FoodItemHistoryEntry
        {
            FoodItemId = e.Data.Id,
            Timestamp = e.Timestamp.UtcDateTime,
            UserId = e.UserName,
            Description = $"{e.Data.Quantity} {e.Data.UnitId} {e.Data.Name} Added."
        };
    }
}
