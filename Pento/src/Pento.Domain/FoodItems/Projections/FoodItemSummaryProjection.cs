using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten.Events.Projections;
using Pento.Domain.FoodItems.Events;

namespace Pento.Domain.FoodItems.Projections;

public class FoodItemSummary
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public decimal ConsumedQuantity { get; set; }
    public decimal DonatedQuantity { get; set; }
    public decimal DiscardedQuantity { get; set; }
}
public class FoodItemSummaryProjection : MultiStreamProjection<FoodItemSummary, Guid>
{

    public FoodItemSummaryProjection()
    {
        Identity<FoodItemAdded>(e => e.HouseholdId);
    }
}
