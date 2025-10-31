using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItems;
using Pento.Domain.RecipeMedia;
using Pento.Domain.Recipes;

namespace Pento.Infrastructure.Configurations;

internal sealed class MediaConfiguration : IEntityTypeConfiguration<RecipeMedia>
{
    public void Configure(EntityTypeBuilder<RecipeMedia> builder)
    {
        builder.ToTable("recipe_media");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Url)
               .IsRequired()
               .HasMaxLength(2048);
        builder.Property(m => m.MimeType)
               .HasMaxLength(50);
        builder.HasOne<Recipe>()
               .WithMany(fi => fi.Media)
               .HasForeignKey(m => m.RecipeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
