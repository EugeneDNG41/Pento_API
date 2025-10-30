using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Application.Abstractions.Authentication;
using Pento.Domain.Compartments;
using Pento.Domain.FoodReferences;
using Pento.Domain.StorageItems;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Configurations;

internal sealed class StorageItemConfiguration : IEntityTypeConfiguration<StorageItem>
{
    public void Configure(EntityTypeBuilder<StorageItem> builder)
    {
        builder.ToTable("storage_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.CompartmentId)
               .IsRequired();

        builder.Property(x => x.FoodRefId)
               .IsRequired();

        builder.Property(x => x.UnitId)
               .IsRequired();

        builder.HasOne<Compartment>().WithMany().HasForeignKey(x => x.CompartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<FoodReference>().WithMany().HasForeignKey(x => x.FoodRefId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Unit>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Notes)
           .HasMaxLength(500);

    }
}
