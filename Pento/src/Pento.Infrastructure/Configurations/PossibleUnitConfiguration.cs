using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.PossibleUnits;

namespace Pento.Infrastructure.Configurations;
internal sealed class PossibleUnitConfiguration : IEntityTypeConfiguration<PossibleUnit>
{
    public void Configure(EntityTypeBuilder<PossibleUnit> builder)
    {
        builder.ToTable("possible_units");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FoodReferenceId).IsRequired();
        builder.Property(p => p.UnitId).IsRequired();
        builder.Property(p => p.IsDefault).IsRequired();

        builder.Property(p => p.CreatedOnUtc).IsRequired();
        builder.Property(p => p.UpdatedOnUtc).IsRequired();

        builder.HasIndex(p => new { p.FoodReferenceId, p.UnitId }).IsUnique();
    }
}
