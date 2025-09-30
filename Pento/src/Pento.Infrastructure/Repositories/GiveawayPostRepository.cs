using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;

namespace Pento.Infrastructure.Repositories;

internal sealed class GiveawayPostRepository(ApplicationDbContext context) : Repository<GiveawayClaim>(context), IGiveawayPostRepository
{
}
