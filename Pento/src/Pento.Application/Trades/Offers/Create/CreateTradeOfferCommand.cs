using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.Notifications;

namespace Pento.Application.Trades.Offers.Create;
public sealed record CreateTradeOfferCommand(
    DateTime StartDate,
    DateTime EndDate,
    PickupOption PickupOption
) : ICommand<Guid>;
