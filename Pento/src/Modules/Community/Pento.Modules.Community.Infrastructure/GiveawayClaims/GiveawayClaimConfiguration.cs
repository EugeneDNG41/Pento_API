using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Modules.Community.Domain.GiveawayClaim;

namespace Pento.Modules.Community.Infrastructure.GiveawayClaims;
internal sealed class GiveawayClaimConfiguration : IEntityTypeConfiguration<GiveawayClaim>
{
    public void Configure(EntityTypeBuilder<GiveawayClaim> builder)
    {
        throw new NotImplementedException();
    }
}
