using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayPosts.Get;
internal sealed class GetGiveawayPostQueryHandler() : IQueryHandler<GetGiveawayPostQuery, GiveawayPostResponse>
{
    public Task<Result<GiveawayPostResponse>> Handle(GetGiveawayPostQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

