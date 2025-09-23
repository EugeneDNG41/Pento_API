using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Community.Application.GiveawayPost.Get;
internal sealed class GetGiveawayPostQueryHandler() : IQueryHandler<GetGiveawayPostQuery, GiveawayPostResponse>
{
    public Task<Result<GiveawayPostResponse>> Handle(GetGiveawayPostQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

