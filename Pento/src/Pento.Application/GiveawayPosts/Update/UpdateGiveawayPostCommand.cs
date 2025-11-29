using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Update;
public sealed record UpdateGiveawayPostCommand(
    Guid Id,
    string? TitleDescription,
    string? ContactInfo,
    string? Address,
    decimal? Quantity,
    DateTime? PickupStartDate,
    DateTime? PickupEndDate,
    PickupOption? PickupOption,
    GiveawayStatus Status
) : ICommand<Guid>;
