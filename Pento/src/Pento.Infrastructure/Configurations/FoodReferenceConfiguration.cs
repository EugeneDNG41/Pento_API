using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodReferences;

namespace Pento.Infrastructure.Configurations;
internal sealed class FoodReferenceConfiguration : IEntityTypeConfiguration<FoodReference>
{
    public void Configure(EntityTypeBuilder<FoodReference> builder)
    {
        builder.ToTable("food_references");
        builder.HasKey(fr => fr.Id);

        builder.Property(fr => fr.Name).HasMaxLength(200).IsRequired();
        builder.Property(fr => fr.FoodGroup).HasConversion<string>().IsRequired();
        builder.Property(fr => fr.Barcode).HasMaxLength(50);
        builder.Property(fr => fr.Brand).HasMaxLength(100);
        builder.Property(fr => fr.OpenFoodFactsId).HasMaxLength(100);
        builder.Property(fr => fr.UsdaId).HasMaxLength(100);
    }
}
