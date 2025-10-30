using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.FoodReferences;
using Pento.Domain.Shared;

namespace Pento.Infrastructure.Repositories;
internal sealed class FoodReferenceRepository : Repository<FoodReference>, IFoodReferenceRepository
{
    public FoodReferenceRepository(ApplicationDbContext dbContext)
    : base(dbContext)
    {
    }

    public async Task<FoodReference?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<FoodReference>()
            .FirstOrDefaultAsync(f => f.Name == name, cancellationToken);
    }

    public async Task<FoodReference?> GetByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<FoodReference>()
            .FirstOrDefaultAsync(f => f.Barcode == barcode, cancellationToken);
    }

    public async Task AddAsync(
        FoodReference foodReference,
        CancellationToken cancellationToken = default)
    {
        await DbContext.Set<FoodReference>().AddAsync(foodReference, cancellationToken);
    }
    public async Task<IReadOnlyList<FoodReference>> GetAllWithoutImageAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await DbContext.FoodReferences
           .Where(f => f.ImageUrl == null)
           .OrderBy(f => f.CreatedOnUtc)
           .Take(limit)
           .ToListAsync(cancellationToken);
    }
}
