using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Modules.Community.Domain.GiveawayPost;

namespace Pento.Modules.Community.Infrastructure.GiveawayPosts;
internal sealed class GiveawayPostConfiguration : IEntityTypeConfiguration<GiveawayPost>
{
    public void Configure(EntityTypeBuilder<GiveawayPost> builder)
    {
        throw new NotImplementedException();
    }
}
