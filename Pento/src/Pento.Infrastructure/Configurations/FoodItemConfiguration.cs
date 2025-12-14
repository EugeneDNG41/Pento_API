using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Units;
using Pento.Domain.Users;

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
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(s => s.AddedBy)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany<FoodItemReservation>()
            .WithOne()
            .HasForeignKey(x => x.FoodItemId);
        builder.HasMany<TradeItem>()
            .WithOne()
            .HasForeignKey(x => x.FoodItemId);
        builder.HasMany<TradeSessionItem>()
            .WithOne()
            .HasForeignKey(x => x.FoodItemId);
        builder.Property(x => x.Notes)
           .HasMaxLength(500);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property<uint>("Version").IsRowVersion();
    }
}
