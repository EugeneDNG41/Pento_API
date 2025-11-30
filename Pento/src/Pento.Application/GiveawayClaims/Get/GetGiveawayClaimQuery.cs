using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Giveaways.Claims.Get;


namespace Pento.Application.GiveawayClaims.Get;
public sealed record GetGiveawayClaimQuery(Guid ClaimId): IQuery<GiveawayClaimDetailResponse>;

