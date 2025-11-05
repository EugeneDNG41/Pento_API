using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.DietaryTags;
using Pento.Domain.RecipeDietaryTags;

namespace Pento.Infrastructure.Configurations;
public sealed class RecipeDietaryTagConfiguration : IEntityTypeConfiguration<RecipeDietaryTag>
{
    public void Configure(EntityTypeBuilder<RecipeDietaryTag> builder)
    {
        builder.ToTable("recipe_dietary_tags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.RecipeId, x.DietaryTagId })
            .IsUnique();

        builder.Property(x => x.RecipeId)
            .IsRequired();

        builder.Property(x => x.DietaryTagId)
            .IsRequired();

        builder.HasOne<DietaryTag>()
            .WithMany()
            .HasForeignKey(x => x.DietaryTagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
