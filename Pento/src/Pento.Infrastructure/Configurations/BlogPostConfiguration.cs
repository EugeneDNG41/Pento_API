using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.BlogPosts;
using Pento.Domain.Payments;
using Pento.Domain.Shared;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;
internal sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("blog_posts");
        builder.HasKey(bp => bp.Id);
        builder.Property(bp => bp.Content).HasMaxLength(2000).HasConversion(content => content.Value, value => Content.Create(value));

        builder.HasOne<User>().WithMany().HasForeignKey(bp => bp.UserId);
        builder.HasQueryFilter(c => !c.IsDeleted);

    }
}
#pragma warning disable S125 // Sections of code should not be commented out
//internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
//{
//    public void Configure(EntityTypeBuilder<Payment> builder)
//    {
//        builder.ToTable("payments");
//        builder.HasKey(p => p.Id);
//        builder.Property(p => p.OrderCode).ValueGeneratedOnAdd();
//        builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);
//        builder.HasQueryFilter(c => !c.IsDeleted);
//    }
//}
