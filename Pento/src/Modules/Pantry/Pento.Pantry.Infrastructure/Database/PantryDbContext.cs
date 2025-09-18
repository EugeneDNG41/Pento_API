using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Modules.Pantry.Application.Abstractions.Data;
using Pento.Modules.Pantry.Domain.FoodItems;
using Pento.Modules.Pantry.Domain.Storages;

namespace Pento.Modules.Pantry.Infrastructure.Database;

public sealed class PantryDbContext(DbContextOptions<PantryDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<FoodItem> FoodItems { get; set; }
    internal DbSet<Storage> Storages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Pantry);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PantryDbContext).Assembly);
    }
}
