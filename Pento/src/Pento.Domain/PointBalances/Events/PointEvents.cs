using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances.Events;

public record BalanceCreated //create when first get by id or first point added
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid UserId { get; init; }
}
public record PointAdded
{
    public Guid TaskId { get; init; }
    public PointCategory Category { get; init; }
    public string TaskName { get; init; }
    public int Amount { get; init; }
}
