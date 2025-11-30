using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Recipes;

namespace Pento.Infrastructure.Configurations;
internal sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("recipes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.Servings);

        builder.Property(r => r.CaloriesPerServing);

        builder.Property(r => r.CreatedBy);

        builder.Property(r => r.IsPublic)
            .IsRequired();

        builder.OwnsOne(r => r.RecipeTime, time =>
        {
            time.Property(t => t.PrepTimeMinutes)
                .HasColumnName("prep_time_minutes")
                .IsRequired();

            time.Property(t => t.CookTimeMinutes)
                .HasColumnName("cook_time_minutes")
                .IsRequired();

            time.Ignore(t => t.TotalMinutes);
        });

        builder.Property(r => r.DifficultyLevel)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.ImageUrl)
            .HasConversion(
                uri => uri != null ? uri.ToString() : null,
                str => !string.IsNullOrWhiteSpace(str) ? new Uri(str) : null
            )
            .HasColumnName("image_url")
            .HasMaxLength(500);

        builder.Property(r => r.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(r => r.UpdatedOnUtc)
            .HasColumnName("updated_on_utc")
            .IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
