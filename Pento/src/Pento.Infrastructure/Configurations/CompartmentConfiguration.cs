using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Storages;

namespace Pento.Infrastructure.Configurations;

internal sealed class CompartmentConfiguration : IEntityTypeConfiguration<Compartment>
{
    public void Configure(EntityTypeBuilder<Compartment> builder)
    {
        builder.ToTable("compartments");
        builder.HasKey(c => c.Id);
        builder.Property(x => x.StorageId)
               .IsRequired();
        builder.Property(c => c.Name)
               .HasMaxLength(100)
               .IsRequired();
        builder.HasOne<Storage>()
               .WithMany()
               .HasForeignKey(c => c.StorageId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Household>()
               .WithMany()
               .HasForeignKey(s => s.HouseholdId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.Property(c => c.Notes)
               .HasMaxLength(500);
        builder.HasQueryFilter(c => !c.IsDeleted && !c.IsArchived);
        builder.Property<uint>("Version").IsRowVersion();
    }
}
