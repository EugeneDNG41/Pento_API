using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.DietaryTags;
using Pento.Domain.UserPreferences;

namespace Pento.Infrastructure.Configurations;
public sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("user_preferences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.DietaryTagId)
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.DietaryTagId })
            .IsUnique();

        builder.HasOne<DietaryTag>()
            .WithMany()
            .HasForeignKey(x => x.DietaryTagId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
    }
}
