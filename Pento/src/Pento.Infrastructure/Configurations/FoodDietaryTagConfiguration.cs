using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.DietaryTags;
using Pento.Domain.FoodDietaryTags;

namespace Pento.Infrastructure.Configurations;
public sealed class FoodDietaryTagConfiguration : IEntityTypeConfiguration<FoodDietaryTag>
{
    public void Configure(EntityTypeBuilder<FoodDietaryTag> builder)
    {
        builder.ToTable("food_dietary_tags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.FoodReferenceId, x.DietaryTagId })
            .IsUnique();

        builder.Property(x => x.FoodReferenceId)
            .IsRequired();

        builder.Property(x => x.DietaryTagId)
            .IsRequired();

        builder.HasOne<DietaryTag>()
            .WithMany()
            .HasForeignKey(x => x.DietaryTagId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
    }
}
