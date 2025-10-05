using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.GiveawayPosts;

namespace Pento.Infrastructure.Configurations;

internal sealed class GiveawayPostConfiguration : IEntityTypeConfiguration<GiveawayPost>
{
    public void Configure(EntityTypeBuilder<GiveawayPost> builder)
    {
        
    }
}
