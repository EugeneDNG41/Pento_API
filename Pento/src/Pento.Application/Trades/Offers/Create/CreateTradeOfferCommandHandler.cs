using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Offers.Create;

internal sealed class CreateTradeOfferCommandHandler(
    IDateTimeProvider dateTimeProvider,
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
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }
        var offer = TradeOffer.Create(
            userId: userId,
            householdId: householdId.Value,
            startDate: command.StartDate,
            endDate: command.EndDate,
            pickupOption: command.PickupOption,
            createdOn: dateTimeProvider.UtcNow
        );

        offerRepository.Add(offer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return offer.Id;
    }
}
