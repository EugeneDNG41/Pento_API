using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayClaims.Get;

internal sealed class GetGiveawayClaimQueryHandler : IQueryHandler<GetGiveawayClaimQuery, GiveawayClaimResponse>
{
    public Task<Result<GiveawayClaimResponse>> Handle(GetGiveawayClaimQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
