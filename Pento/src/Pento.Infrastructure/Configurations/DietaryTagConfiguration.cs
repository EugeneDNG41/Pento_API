using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.DietaryTags;

namespace Pento.Infrastructure.Configurations;

public sealed class DietaryTagConfiguration : IEntityTypeConfiguration<DietaryTag>
{
    public void Configure(EntityTypeBuilder<DietaryTag> builder)
    {
        builder.ToTable("dietary_tags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
