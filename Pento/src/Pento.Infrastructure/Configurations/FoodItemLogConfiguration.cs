using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class FoodItemLogConfiguration : IEntityTypeConfiguration<FoodItemLog>
{
    public void Configure(EntityTypeBuilder<FoodItemLog> builder)
    {
        builder.ToTable("food_item_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FoodItemId)
               .IsRequired();
        builder.Property(x => x.HouseholdId)
               .IsRequired();
        builder.Property(x => x.UserId)
               .IsRequired();
        builder.Property(x => x.UnitId)
                .IsRequired();
        builder.Property(x => x.Timestamp)
               .IsRequired();
        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(10,3)")
               .IsRequired();
        builder.Property(x => x.Action)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<FoodItem>()
               .WithMany()
               .HasForeignKey(x => x.FoodItemId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Household>()
               .WithMany()
               .HasForeignKey(x => x.HouseholdId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(x => x.UnitId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted && !x.IsArchived);
    }
}
