using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Repositories;

internal sealed class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Unit?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Unit>()
            .FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
    }

    public async Task<Unit?> GetByAbbreviationAsync(string abbreviation, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Unit>()
            .FirstOrDefaultAsync(u => u.Abbreviation == abbreviation, cancellationToken);
    }

    public async Task AddAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        await DbContext.AddAsync(unit, cancellationToken);
    }
}
