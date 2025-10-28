using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances.Events;

public record BalanceCreated
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public ClientScope Scope { get; init; }
    public Guid ScopeId { get; init; }
    public Guid PointCategoryId { get; init; }
    public int Amount { get; init; }
}
public record BalanceAdded
{
    public Guid Id { get; init; }
    public int Amount { get; init; }
}
public record BalanceSubtracted
{
    public Guid Id { get; init; }
    public int Amount { get; init; }
}
