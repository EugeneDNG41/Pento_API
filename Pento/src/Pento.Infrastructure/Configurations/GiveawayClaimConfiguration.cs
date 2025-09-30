using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.GiveawayClaims;

namespace Pento.Infrastructure.Configurations;

internal sealed class GiveawayClaimConfiguration : IEntityTypeConfiguration<GiveawayClaim>
{
    public void Configure(EntityTypeBuilder<GiveawayClaim> builder)
    {
        throw new NotImplementedException();
    }
}
