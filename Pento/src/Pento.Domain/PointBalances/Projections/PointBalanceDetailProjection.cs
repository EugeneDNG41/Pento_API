using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.PointBalances.Projections;

public sealed record PointBalanceDetail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid PointCategoryId { get; init; }
    public int Balance { get; init; }
}
internal sealed class PointBalanceDetailProjection
{
}
