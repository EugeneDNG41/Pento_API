using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Compartments;
using Pento.Domain.StorageItems;
using Pento.Domain.Storages;

namespace Pento.Infrastructure.Configurations;

internal sealed class StorageConfiguration : IEntityTypeConfiguration<Storage>
{
    public void Configure(EntityTypeBuilder<Storage> builder)
    {
        builder.ToTable("storages");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name)
               .HasMaxLength(100)
               .IsRequired();
        builder.Property(s => s.Type)
               .HasConversion<string>()
               .HasMaxLength(50);
        builder.Property(s => s.Notes)
               .HasMaxLength(500);
        builder.HasMany<Compartment>()
               .WithOne()
               .HasForeignKey(c => c.StorageId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany<StorageItem>()
               .WithOne()
               .HasForeignKey(s => s.StorageId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
