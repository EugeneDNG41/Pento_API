using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItems;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Configurations;
internal sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(u => u.Type)
               .HasConversion<string>()
               .HasMaxLength(50);
        builder.Property(u => u.Abbreviation)
            .HasMaxLength(20);

        builder.Property(u => u.ToBaseFactor)
            .HasColumnType("decimal(10,3)");
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
    }
}
