using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.DeviceTokens;

namespace Pento.Infrastructure.Configurations;

internal sealed class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("device_tokens");

        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(dt => dt.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(dt => dt.Token)
            .HasColumnName("token")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(dt => dt.Platform)
            .HasColumnName("platform")
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(dt => dt.UserId)
            .HasDatabaseName("ix_device_tokens_user_id");

        builder.HasIndex(dt => dt.Token)
            .IsUnique()
            .HasDatabaseName("ux_device_tokens_token");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
