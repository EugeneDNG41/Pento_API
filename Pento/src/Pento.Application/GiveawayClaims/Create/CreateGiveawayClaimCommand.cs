using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GiveawayClaims.Create;
public sealed record CreateGiveawayClaimCommand(
    Guid GiveawayPostId,
    string? Message
) : ICommand<Guid>;
