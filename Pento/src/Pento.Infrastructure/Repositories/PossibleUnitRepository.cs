using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.PossibleUnits;

namespace Pento.Infrastructure.Repositories;
internal sealed class PossibleUnitRepository : Repository<PossibleUnit>, IPossibleUnitRepository
{
    public PossibleUnitRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<PossibleUnit>> GetByFoodRefIdAsync(
        Guid foodRefId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<PossibleUnit>()
            .Where(p => p.FoodReferenceId == foodRefId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PossibleUnit?> GetDefaultForFoodRefAsync(
        Guid foodRefId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<PossibleUnit>()
            .FirstOrDefaultAsync(p => p.FoodReferenceId == foodRefId && p.IsDefault, cancellationToken);
    }

    public async Task AddAsync(PossibleUnit possibleUnit, CancellationToken cancellationToken = default)
    {
        await DbContext.AddAsync(possibleUnit, cancellationToken);
    }

    public async Task UpdateAsync(PossibleUnit possibleUnit, CancellationToken cancellationToken = default)
    {
        DbContext.Update(possibleUnit);
        await Task.CompletedTask;
    }
}
