using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodReferences;
using Pento.Domain.FoodItems;

namespace Pento.Infrastructure.Configurations;
internal sealed class FoodReferenceConfiguration : IEntityTypeConfiguration<FoodReference>
{
    public void Configure(EntityTypeBuilder<FoodReference> builder)
    {
        builder.ToTable("food_references");
        builder.HasKey(fr => fr.Id);
        builder.Property(fr => fr.Id).HasColumnName("id");
        builder.Property(fr => fr.Name).HasColumnName("name");
        builder.Property(fr => fr.FoodGroup).HasColumnName("food_group");
        builder.Property(fr => fr.Barcode).HasColumnName("barcode");
        builder.Property(fr => fr.Brand).HasColumnName("brand");
        builder.Property(fr => fr.TypicalShelfLifeDays_Pantry)
              .HasColumnName("typical_shelf_life_days_pantry")
              .HasDefaultValue(0);

        builder.Property(fr => fr.TypicalShelfLifeDays_Fridge)
            .HasColumnName("typical_shelf_life_days_fridge")
            .HasDefaultValue(0);

        builder.Property(fr => fr.TypicalShelfLifeDays_Freezer)
            .HasColumnName("typical_shelf_life_days_freezer")
            .HasDefaultValue(0);
        builder.Property(fr => fr.AddedBy).HasColumnName("added_by");
        builder.Property(fr => fr.UsdaId).HasColumnName("usda_id");
        builder.Property(fr => fr.CreatedOnUtc).HasColumnName("created_on_utc");
        builder.Property(fr => fr.UpdatedOnUtc).HasColumnName("updated_on_utc");
        builder.Property(fr => fr.FoodCategoryId).HasColumnName("food_category_id");
        builder.Property(fr => fr.ImageUrl).HasColumnName("image_url");

        builder.Property(fr => fr.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(fr => fr.FoodGroup)
             .HasConversion<string>()      
             .HasMaxLength(50)
             .IsRequired();


        builder.Property(fr => fr.FoodCategoryId);
    
        builder.Property(fr => fr.Brand)
            .HasMaxLength(200);

        builder.Property(fr => fr.Barcode)
            .HasMaxLength(100);

        builder.Property(fr => fr.UsdaId)
            .HasMaxLength(100)
            .IsRequired();



        builder.Property(fr => fr.ImageUrl)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => string.IsNullOrEmpty(v) ? null : new Uri(v))
            .HasMaxLength(500);
            builder.Property(fr => fr.UnitType) 
        .HasColumnName("unit_type")
        .HasConversion<string>()
        .HasMaxLength(50)
        .IsRequired();

        builder.Property(fr => fr.CreatedOnUtc)
            .IsRequired();
        builder.Property(fr => fr.AddedBy);
        builder.Property(fr => fr.UpdatedOnUtc)
            .IsRequired();
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);

    }
}
