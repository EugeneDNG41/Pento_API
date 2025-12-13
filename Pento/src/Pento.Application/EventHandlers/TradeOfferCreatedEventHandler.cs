using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Trades;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers;

internal sealed class TradeOfferCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IUnitOfWork unitOfWork
    )
    : DomainEventHandler<TradeOfferCreatedDomainEvent>
{
    public override async Task Handle(
        TradeOfferCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(domainEvent.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeOfferCreatedEventHandler), TradeErrors.RequestNotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            offer.UserId,
            offer.HouseholdId,
            ActivityCode.TRADE_OFFER_CREATE.ToString(),
            offer.Id,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(TradeOfferCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(TradeOfferCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
