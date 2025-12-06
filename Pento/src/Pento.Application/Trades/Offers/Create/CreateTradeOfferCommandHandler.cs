using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.Create;

internal sealed class CreateTradeOfferCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeOffer> offerRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateTradeOfferCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTradeOfferCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        var offer = new TradeOffer(
            id: Guid.NewGuid(),
            userId: userId,
            status: GiveawayStatus.Open,
            startDate: command.StartDate,
            endDate: command.EndDate,
            pickupOption: command.PickupOption,
            createdOn: DateTime.UtcNow
        );

        offerRepository.Add(offer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return offer.Id;
    }
}
