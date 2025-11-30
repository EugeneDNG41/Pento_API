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
using Pento.Domain.Households;
using Pento.Domain.FoodItems;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Configurations;

internal sealed class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
{
    public void Configure(EntityTypeBuilder<FoodItem> builder)
    {
        builder.ToTable("food_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(10,3)")
               .IsRequired();

        builder.Property(x => x.CompartmentId)
               .IsRequired();

        builder.Property(x => x.FoodReferenceId)
               .IsRequired();

        builder.Property(x => x.UnitId)
               .IsRequired();

        builder.Property(x => x.ExpirationDate)
               .IsRequired();
        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.HouseholdId)
               .IsRequired();

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();
        builder.Property(x => x.ImageUrl)
               .HasMaxLength(500);

        builder.HasOne<Compartment>()
            .WithMany()
            .HasForeignKey(x => x.CompartmentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<FoodReference>()
            .WithMany()
            .HasForeignKey(x => x.FoodReferenceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Household>()
               .WithMany()
               .HasForeignKey(s => s.HouseholdId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Notes)
           .HasMaxLength(500);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property<uint>("Version").IsRowVersion();
    }
}
