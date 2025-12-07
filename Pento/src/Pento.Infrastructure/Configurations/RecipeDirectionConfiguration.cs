using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.RecipeDirections;

namespace Pento.Infrastructure.Configurations;

internal sealed class RecipeDirectionConfiguration : IEntityTypeConfiguration<RecipeDirection>
{
    public void Configure(EntityTypeBuilder<RecipeDirection> builder)
    {
        builder.ToTable("recipe_directions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecipeId).IsRequired();
        builder.Property(x => x.StepNumber).IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => string.IsNullOrWhiteSpace(v) ? null : new Uri(v))
            .HasColumnName("image_url");

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedOnUtc)
            .HasColumnName("updated_on_utc")
            .IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
