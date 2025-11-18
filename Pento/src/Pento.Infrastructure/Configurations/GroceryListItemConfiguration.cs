using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.GroceryListItems;
using Pento.Domain.GroceryLists;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class GroceryListItemConfiguration : IEntityTypeConfiguration<GroceryListItem>
{
    public void Configure(EntityTypeBuilder<GroceryListItem> builder)
    {
        builder.ToTable("grocery_list_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ListId)
            .HasColumnName("list_id")
            .IsRequired();

        builder.Property(x => x.FoodRefId)
            .HasColumnName("food_ref_id")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.CustomName)
            .HasColumnName("custom_name")
            .HasMaxLength(200);

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id");



        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        builder.Property(x => x.Priority)
            .HasColumnName("priority")
            .HasConversion<string>() //("Low", "Medium", "High")
            .IsRequired();

        builder.Property(x => x.AddedBy)
            .HasColumnName("added_by")
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.HasIndex(x => new { x.ListId, x.FoodRefId });

        builder.HasOne<GroceryList>()
            .WithMany()
            .HasForeignKey(x => x.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<FoodReference>()
            .WithMany()
            .HasForeignKey(x => x.FoodRefId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AddedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
