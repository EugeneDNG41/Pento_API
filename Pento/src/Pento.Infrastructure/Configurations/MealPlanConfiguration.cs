using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

namespace Pento.Infrastructure.Configurations;
internal sealed class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("meal_plans");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.HouseholdId)
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(mp => mp.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(mp => mp.RecipeId)
            .IsRequired();
                builder.Property(mp => mp.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(mp => mp.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(mp => mp.MealType)
            .IsRequired()
            .HasConversion<int>(); 

        builder.Property(mp => mp.ScheduledDate)
            .HasColumnName("scheduled_date")
            .IsRequired();

        builder.Property(mp => mp.Servings)
            .IsRequired();

        builder.Property(mp => mp.Notes)
            .HasMaxLength(500);

        builder.Property(mp => mp.CreatedBy)
            .IsRequired();

        builder.Property(mp => mp.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(mp => mp.UpdatedOnUtc)
            .HasColumnName("updated_on_utc")
            .IsRequired();
        builder.HasQueryFilter(c => !c.IsDeleted);

    }
}
