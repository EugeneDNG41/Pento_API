using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Notifications;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.Body)
            .HasColumnName("body")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(n => n.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion<string>();

        builder.Property(n => n.DataJson)
            .HasColumnName("data_json")
            .HasColumnType("jsonb")
            .IsRequired(false);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => n.UserId)
            .HasDatabaseName("ix_notifications_user_id");


    }
}
