using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.MealPlans;

namespace Pento.Infrastructure.Configurations;
internal sealed class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("meal_plans");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.HouseholdId)
            .IsRequired();

        builder.Property(mp => mp.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(mp => mp.Duration, duration =>
        {
            duration.Property(d => d.Start)
                .HasColumnName("start_date")
                .IsRequired();

            duration.Property(d => d.End)
                .HasColumnName("end_date")
                .IsRequired();
        });

        builder.Property(mp => mp.CreatedBy)
            .IsRequired();

        builder.Property(mp => mp.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(mp => mp.UpdatedOnUtc)
            .HasColumnName("updated_on_utc")
            .IsRequired();
    }
}
